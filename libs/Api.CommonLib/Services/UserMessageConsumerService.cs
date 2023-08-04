using Api.CommonLib.Interfaces;
using Api.MessageLib.Models;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Api.UserLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.CommonLib.Services
{
    public class UserMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<UserMessageConsumerService> _logger;
        private readonly IMongoCollection<BsonDocument> _userCols;
        private readonly ILineGroupInfo _lineUserInfo;
        private readonly IUserRepository _userRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IScopePublisher _publisher;
        private readonly IUserChatRepository _userChatRepo;
        public UserMessageConsumerService(ILogger<UserMessageConsumerService> logger,
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
                LatestMessage = new UserLatestMessageModel
                {
                    IsRead = false,
                    MessageId = msgModel.MessageId
                }
            };

            // check if user exist
            Response userResponse = _userRepo.FindUser(userModel.GroupUserId!);
            if (userResponse.StatusCode == StatusCodes.Status409Conflict)
            {
                return;
            }

            var userInfo = await _lineUserInfo.GetGroupMemberProfile(msgModel.GroupId!, userModel.GroupUserId!, _channelSetting.Value.ChannelAccessToken!);

            userModel.DisplayName = userInfo.DisplayName;
            userModel.PictureUrl = userInfo.PictureUrl;
            userResponse = _userRepo.AddUser(userModel);

            // add the user chat
            UserChatModel userChatModel = new UserChatModel{
                GroupId=msgModel.GroupId,
                GroupUserId=msgModel.GroupUserId
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
            return;
        }
    }
}