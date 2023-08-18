using Api.AuthLib.Models;
using Api.ChatLib.Interfaces;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.PlatformLib.DTOs;
using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.CommonLib.Consumers
{
    public class UserConsumer : IUserConsumer
    {
        private readonly ILogger<UserConsumer> _logger;
        private readonly IMongoCollection<BsonDocument> _userCols;
        private readonly ILineGroupInfo _lineUserInfo;
        private readonly IUserRepository _userRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageGrpcClientService _msgGrpc;
        private readonly IChatGrpcClientService _chatGrpc;
        private readonly IMessagePublisher _publisher;
        public UserConsumer(ILogger<UserConsumer> logger,
            IOptions<MongoConfigSetting> mongoConfig,
            ILineGroupInfo lineUserInfo,
            IUserRepository userRepo,
            IOptions<LineChannelSetting> channelSetting,
            IUserChatRepository userChatRepo,
            IMessageGrpcClientService msgGrpc,
            IChatGrpcClientService chatGrpc,
            IServiceProvider serviceProvider,
            IMessagePublisher publisher)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["User"]);
            _lineUserInfo = lineUserInfo;
            _userRepo = userRepo;
            _channelSetting = channelSetting;
            _userChatRepo = userChatRepo;
            _msgGrpc = msgGrpc;
            _chatGrpc = chatGrpc;
            _publisher = publisher;
        }

        public async Task ConsumeAuthUpdate(string message)
        {
            LineAuthStateModel authModel = JsonConvert
                .DeserializeObject<LineAuthStateModel>(message)!;

            UserModel userModel = new UserModel();
            try
            {
                userModel = await _userRepo
                    .FindUser(authModel.GroupUserId!);
            }
            catch
            {
                userModel = new UserModel
                {
                    Token = authModel.Token,
                    GroupUserId = authModel.GroupUserId,
                    DisplayName = authModel.DisplayName,
                    PictureUrl = authModel.PictureUrl,
                    StatusMessage = authModel.StatusMessage,
                    Platform = new UserPlatformModel
                    {
                        PlatformId = authModel.PlatformId
                    }
                };
            }

            await _userRepo.AddUser(userModel);

            // publish the user
            string pubMessage = JsonConvert.SerializeObject(userModel);
            string routingKey = RoutingKeys.User["create"];

            try
            {
                _publisher.Publish(pubMessage, routingKey, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                _publisher.Dispose();
            }
            return;
        }

        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            // _logger.LogInformation(message);
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;

            ChatModel chatModel = new ChatModel();
            try
            {
                chatModel = await _chatGrpc.GetChat(msgModel.GroupId!);
            }
            catch (ErrorResponseException ex)
            {
                _logger.LogError(ex.Description);
                return;
            }

            UserModel userModel = new UserModel
            {
                ClientId = msgModel.ClientId,
                GroupUserId = msgModel.GroupUserId,
                Platform = new UserPlatformModel
                {
                    PlatformId = chatModel.PlatformId
                }
            };

            // check if user exist
            UserModel existingUser = new UserModel();

            bool isUserExist = false;
            try
            {
                existingUser = await _userRepo.FindUser(userModel.GroupUserId!);
                isUserExist = true;
            }
            // if user not exist add it.
            catch (ErrorResponseException ex)
            {
                _logger.LogWarning($"{ex.Description}\nAdding new user...");
                isUserExist = false;
                var userInfo = new GetGroupMemberProfileDto();
                try
                {
                    userInfo = await _lineUserInfo.GetGroupMemberProfile(msgModel.GroupId!, userModel.GroupUserId!, _channelSetting.Value.ChannelAccessToken!);
                }
                catch (Exception lineEx)
                {
                    _logger.LogError(lineEx.Message);
                    return;
                }

                // get the chat using grpc
                // assign the platform_id to both user and user_chat
                userModel.DisplayName = userInfo.DisplayName;
                userModel.PictureUrl = userInfo.PictureUrl;

                await _userRepo.AddUser(userModel);
            }

            // add the user chat
            UserChatModel userChatModel = new UserChatModel
            {
                GroupId = msgModel.GroupId,
                GroupUserId = msgModel.GroupUserId,
                PlatformId = chatModel.PlatformId
            };

            bool isUserChatExist = false;

            try
            {
                isUserChatExist = true;
                userChatModel = await _userChatRepo.FindUserChat(chatModel.Group!.GroupId!, msgModel.GroupUserId!);
                return;
            }
            catch (ErrorResponseException ex)
            {
                _logger.LogWarning($"{ex.Description}\nAdding new user chat...");
                await _userChatRepo.AddUserChat(userChatModel);
                isUserChatExist = false;
            }

            try
            {
                string pubMessage = JsonConvert.SerializeObject(userModel);
                string routingKey = RoutingKeys.User["create"];

                if (!isUserExist)
                {
                    // publish the user
                    _publisher.Publish(pubMessage, routingKey, null);
                }
                if (!isUserChatExist)
                {
                    // publish the user
                    pubMessage = JsonConvert.SerializeObject(userChatModel);
                    routingKey = RoutingKeys.UserChat["create"];
                    _publisher.Publish(pubMessage, routingKey, null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                _publisher.Dispose();
            }
            return;
        }

        public async Task ConsumeMessageVerify(string message)
        {
            MessageModel messageModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;
            UserModel existingUser = new UserModel();
            UserChatModel userChat = new UserChatModel();
            string resMsg = "";

            try
            {
                existingUser = await _userRepo.FindUser(messageModel.GroupUserId!);
            }
            catch
            {
                resMsg = "No admin user exit.";
                _logger.LogError(resMsg);
                return;
            }

            try
            {
                userChat = await _userChatRepo.FindUserChatByGroupId(messageModel.GroupId!);
            }
            catch
            {
                if (!existingUser.Platform!.IsVerified)
                {
                    resMsg = "Need to verify the platform first!";
                    _logger.LogError(resMsg);
                    return;
                }

                userChat = new UserChatModel
                {
                    GroupId = messageModel.GroupId,
                    PlatformId = existingUser.Platform!.PlatformId,
                };

                await _userChatRepo.AddUserChat(userChat);


                try
                {
                    // add the message
                    messageModel.PlatformId = existingUser.Platform.PlatformId;
                    await _msgGrpc.AddMessage(messageModel);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }

                // publish the user chat
                string strUserChat = JsonConvert.SerializeObject(userChat);
                string routingKey = RoutingKeys.UserChat["verify"];
                try
                {
                    _publisher.Publish(strUserChat, routingKey, null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                finally
                {
                    _publisher.Dispose();
                }
                return;
            }

            resMsg = "User chat exist!";
            _logger.LogWarning(resMsg);
            return;
        }

        public async Task ConsumePlatformVerify(string message)
        {
            _logger.LogInformation(message);
            PlatformDto platformDto = JsonConvert
                .DeserializeObject<PlatformDto>(message)!;

            try
            {
                // find and replace the user
                UserModel userModel = new UserModel();
                DateTime modifiedDate = userModel.ModifiedDate;

                userModel = await _userRepo.FindUser(platformDto.GroupUserId!);
                userModel.ModifiedDate = modifiedDate;
                userModel.Platform = new UserPlatformModel
                {
                    PlatformId = platformDto.PlatformId,
                    IsVerified = true
                };
                await _userRepo.ReplaceUser(userModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return;
            }

        }
    }
}