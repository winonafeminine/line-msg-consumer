using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.CommonLib.Interfaces;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.PlatformLib.DTOs;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.CommonLib.Consumers
{
    public class UserConsumer : IUserConsumer
    {
        private readonly ILogger<UserConsumer> _logger;
        private readonly IMongoCollection<BsonDocument> _userCols;
        private readonly ILineGroupInfo _lineUserInfo;
        private readonly IUserRepository _userRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IScopePublisher _publisher;
        private readonly IUserChatRepository _userChatRepo;
        private readonly IMessageGrpcClientService _msgGrpc;
        public UserConsumer(ILogger<UserConsumer> logger,
            IOptions<MongoConfigSetting> mongoConfig,
            ILineGroupInfo lineUserInfo,
            IUserRepository userRepo,
            IOptions<LineChannelSetting> channelSetting,
            IScopePublisher publisher,
            IUserChatRepository userChatRepo,
            IMessageGrpcClientService msgGrpc)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["User"]);
            _lineUserInfo = lineUserInfo;
            _userRepo = userRepo;
            _channelSetting = channelSetting;
            _publisher = publisher;
            _userChatRepo = userChatRepo;
            _msgGrpc = msgGrpc;
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
            _publisher.Publish(pubMessage, routingKey, null);
            return;
        }

        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            // _logger.LogInformation(message);
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;

            UserModel userModel = new UserModel
            {
                ClientId = msgModel.ClientId,
                GroupUserId = msgModel.GroupUserId,
            };

            // check if user exist
            UserModel existingUser = new UserModel();
            try
            {
                existingUser = await _userRepo.FindUser(userModel.GroupUserId!);
                if (existingUser != null)
                {
                    _logger.LogInformation("Do nothing");
                    // need to replace the user chat
                    // publish the user chat
                    return;
                }
            }
            finally
            {
                var userInfo = await _lineUserInfo.GetGroupMemberProfile(msgModel.GroupId!, userModel.GroupUserId!, _channelSetting.Value.ChannelAccessToken!);

                // get the chat using grpc
                // assign the platform_id to both user and user_chat
                userModel.DisplayName = userInfo.DisplayName;
                userModel.PictureUrl = userInfo.PictureUrl;
                await _userRepo.AddUser(userModel);

                // add the user chat
                UserChatModel userChatModel = new UserChatModel
                {
                    GroupId = msgModel.GroupId,
                    GroupUserId = msgModel.GroupUserId
                };

                await _userChatRepo.AddUserChat(userChatModel);

                // publish the user
                string pubMessage = JsonConvert.SerializeObject(userModel);
                string routingKey = RoutingKeys.User["create"];
                _publisher.Publish(pubMessage, routingKey, null);

                // publish the user
                pubMessage = JsonConvert.SerializeObject(userChatModel);
                routingKey = RoutingKeys.UserChat["create"];
                _publisher.Publish(pubMessage, routingKey, null);
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

                // publish the user chat
                string strUserChat = JsonConvert.SerializeObject(userChat);
                string routingKey = RoutingKeys.UserChat["verify"];
                _publisher.Publish(strUserChat, routingKey, null);

                try{
                    // add the message
                    messageModel.PlatformId=existingUser.Platform.PlatformId;
                    await _msgGrpc.AddMessage(messageModel);
                }catch (Exception ex){
                    _logger.LogError(ex.Message);
                }

                return;
            }
            
            resMsg="User chat exist!";
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