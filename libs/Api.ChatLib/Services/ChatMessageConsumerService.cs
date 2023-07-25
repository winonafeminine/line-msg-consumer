using Api.ChatLib.DTOs;
using Api.ChatLib.Models;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.ChatLib.Services
{
    public class ChatMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<ChatMessageConsumerService> _logger;
        private readonly IMongoCollection<BsonDocument> _chatCols;
        public ChatMessageConsumerService(ILogger<ChatMessageConsumerService> logger, IOptions<ChatMongoConfigModel> mongoConfig)
        {
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _chatCols = mongodb.GetCollection<BsonDocument>(mongoConfig.Value.Collections!.Chat);
        }
        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;

            ChatModel chatModel = new ChatModel
            {
                Group = new ChatGroupDto{
                    GroupId=msgModel.GroupId
                },
                LatestMessage = new ChatLatestMessageDto
                {
                    MessageId = msgModel.MessageId,
                    GroupUserId = msgModel.GroupUserId,
                    LatestDate = msgModel.CreatedDate,
                    // get the text from message object
                }
            };

            await _chatCols.InsertOneAsync(
                BsonDocument.Parse(JsonConvert.SerializeObject(chatModel))
            );
            
            _logger.LogInformation($"Chat saved!");
            return;
        }
    }
}