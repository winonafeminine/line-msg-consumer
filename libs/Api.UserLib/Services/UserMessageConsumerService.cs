using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Api.CommonLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.UserLib.Services
{
    public class UserMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<UserMessageConsumerService> _logger;
        private readonly IMongoCollection<BsonDocument> _userCols;
        private readonly ILineGroupInfo _lineUserInfo;
        private readonly IUserRepository _userRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IServiceProvider _serviceProvider;
        public UserMessageConsumerService(ILogger<UserMessageConsumerService> logger,
            IOptions<MongoConfigSetting> mongoConfig,
            ILineGroupInfo lineUserInfo,
            IUserRepository userRepo,
            IOptions<LineChannelSetting> channelSetting,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["User"]);
            _lineUserInfo = lineUserInfo;
            _userRepo = userRepo;
            _channelSetting = channelSetting;
            _serviceProvider = serviceProvider;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            // _logger.LogInformation(message);
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;
            
            UserModel userModel = new UserModel{
                ClientId=msgModel.ClientId,
                GroupId=msgModel.GroupId,
                GroupUserId=msgModel.GroupUserId,
                LatestMessage=new UserLatestMessageModel{
                    IsRead=false,
                    MessageId=msgModel.MessageId
                }
            };

            // check if user exist
            Response userResponse = _userRepo.FindUser(userModel.GroupUserId!);
            if(userResponse.StatusCode == StatusCodes.Status409Conflict)
            {
                return;
            }

            var userInfo = await _lineUserInfo.GetGroupMemberProfile(userModel.GroupId!, userModel.GroupUserId!, _channelSetting.Value.ChannelAccessToken!);

            userModel.DisplayName = userInfo.DisplayName;
            userModel.PictureUrl = userInfo.PictureUrl;
            userResponse = _userRepo.AddUser(userModel);

            // publish the user
            using(var scope = _serviceProvider.CreateScope())
            {
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
                string routingKey = RoutingKeys.User["create"];
                string strUserModel = JsonConvert.SerializeObject(userModel);
                publisher.Publish(strUserModel, routingKey, null);
            }
            return;
        }
    }
}