using Api.CommonLib.Interfaces;
using Api.MessageLib.Models;
using Api.PlatformLib.DTOs;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Microsoft.AspNetCore.Http;
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
        public UserConsumer(ILogger<UserConsumer> logger,
            IOptions<MongoConfigSetting> mongoConfig,
            ILineGroupInfo lineUserInfo,
            IUserRepository userRepo,
            IOptions<LineChannelSetting> channelSetting,
            IScopePublisher publisher,
            IUserChatRepository userChatRepo)
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
                    return;
                }
            }
            finally
            {
                var userInfo = await _lineUserInfo.GetGroupMemberProfile(msgModel.GroupId!, userModel.GroupUserId!, _channelSetting.Value.ChannelAccessToken!);

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

        public async Task ConsumePlatformVerify(string message)
        {
            PlatformDto platformDto = JsonConvert
                .DeserializeObject<PlatformDto>(message)!;

            try
            {
                // find and replace the user
                UserModel userModel = new UserModel();
                DateTime modifiedDate = userModel.ModifiedDate;

                userModel = await _userRepo.FindUser(platformDto.GroupUserId!);
                userModel.ModifiedDate = modifiedDate;
                userModel.PlatformId = platformDto.PlatformId;
                await _userRepo.ReplaceUser(userModel);
            }
            catch
            {
                _logger.LogError("Failed update the user platform_id");
                return;
            }

        }
    }
}