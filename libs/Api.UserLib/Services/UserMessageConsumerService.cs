using Api.CommonLib.Interfaces;
using Api.CommonLib.Stores;
using Api.MessageLib.Models;
using Api.MessageLib.RPCs;
using Api.MessageLib.Settings;
using Api.UserLib.DTOs;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
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
        private readonly MessageRpcClient _messageRpc;
        private readonly ILineUserInfo _lineUserInfo;
        public UserMessageConsumerService(ILogger<UserMessageConsumerService> logger,
            IOptions<UserMongoConfigModel> mongoConfig,
            MessageRpcClient messageRpc,
            ILineUserInfo lineUserInfo)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(mongoConfig.Value.Collections!.User);
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
                LatestMessage=new UserLatestMessageDto{
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
                _logger.LogError("RPC error");
            }

            var userInfo = await _lineUserInfo.GetGroupMemberProfile(userModel.GroupId!, userModel.GroupUserId!, response.ChannelAccessToken!);

            userModel.DisplayName = userInfo.DisplayName;
            userModel.LineUserId = userInfo.UserId;
            userModel.PictureUrl = userInfo.PictureUrl;

            await _userCols.InsertOneAsync(
                BsonDocument.Parse(JsonConvert.SerializeObject(userModel))
            );

            _logger.LogInformation($"New User saved!");
            return;
        }
    }
}