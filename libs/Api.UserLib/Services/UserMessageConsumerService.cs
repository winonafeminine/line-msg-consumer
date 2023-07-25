using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Api.CommonLib.Stores;
using Api.UserLib.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.UserLib.Services
{
    public class UserMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<UserMessageConsumerService> _logger;
        private readonly IMongoCollection<BsonDocument> _userCols;
        private readonly IMessageRpcClient _messageRpc;
        private readonly ILineUserInfo _lineUserInfo;
        public UserMessageConsumerService(ILogger<UserMessageConsumerService> logger,
            IOptions<MongoConfigSetting> mongoConfig,
            IMessageRpcClient messageRpc,
            ILineUserInfo lineUserInfo)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["User"]);
            _messageRpc = messageRpc;
            _lineUserInfo = lineUserInfo;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;
            
            var users = _userCols.Find(x=>x["group_user_id"] == msgModel.GroupUserId)
                .ToEnumerable();
            
            if(users.Any())
            {
                return;
            }
            
            UserModel userModel = new UserModel{
                ClientId=msgModel.ClientId,
                GroupId=msgModel.GroupId,
                GroupUserId=msgModel.GroupUserId,
                LatestMessage=new UserLatestMessageModel{
                    IsRead=false,
                    MessageId=msgModel.MessageId
                }
            };

            var response = new LineChannelSetting();

            try{
                string messageQueue = RpcQueueNames.Message;
                response = _messageRpc.GetChannel();
                // _logger.LogInformation(JsonConvert.SerializeObject(response));
            }catch{
                _logger.LogError("Failed getting channel");
            }

            var userInfo = await _lineUserInfo.GetGroupMemberProfile(userModel.GroupId!, userModel.GroupUserId!, response.ChannelAccessToken!);

            userModel.DisplayName = userInfo.DisplayName;
            userModel.LineUserId = userInfo.UserId;
            userModel.PictureUrl = userInfo.PictureUrl;

            await _userCols.InsertOneAsync(
                BsonDocument.Parse(JsonConvert.SerializeObject(userModel))
            );

            // create user in APi.MessageSv
            Response addUserResponse = new Response();
            try{
                addUserResponse = _messageRpc.AddUser(userModel);
            }catch{
                _logger.LogError("Failed adding user");
            }
            _logger.LogInformation($"New User saved!");
            return;
        }
    }
}