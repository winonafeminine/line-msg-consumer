using Api.CommonLib.Interfaces;
using Api.CommonLib.Stores;
using Api.MessageLib.Models;
using Api.MessageLib.RPCs;
using Api.MessageLib.Settings;
using Api.UserLib.DTOs;
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
        public UserMessageConsumerService(ILogger<UserMessageConsumerService> logger,
            IOptions<UserMongoConfigModel> mongoConfig,
            MessageRpcClient messageRpc)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(mongoConfig.Value.Collections!.User);
            _messageRpc = messageRpc;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;
            
            UserModel userModel = new UserModel{
                ClientId=msgModel.ClientId,
                GroupId=msgModel.GroupId,
                LineUserId=msgModel.LineUserId,
                LatestMessage=new UserLatestMessageDto{
                    IsRead=false,
                    MessageId=msgModel.MessageId
                }
            };

            await _userCols.InsertOneAsync(
                BsonDocument.Parse(JsonConvert.SerializeObject(userModel))
            );
            _logger.LogInformation($"User saved!");

            try{
                IDictionary<string, string> messageQueue = RpcQueueNames.Message;
                var response = _messageRpc.SendRPCRequest(
                    JsonConvert.SerializeObject(userModel), 
                    messageQueue["GetChannel"]
                );

                // var response = await rpcClient.CallAsync("100");
                _logger.LogInformation(response);
            }catch{
                _logger.LogError("RPC error");
            }
            return;
        }
    }
}