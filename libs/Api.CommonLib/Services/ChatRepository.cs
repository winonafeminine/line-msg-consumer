using Api.CommonLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.CommonLib.Services
{
    public class ChatRepository : IChatRepository
    {
        private readonly IMongoCollection<BsonDocument> _chatCols;
        private readonly IOptions<MongoConfigSetting> _mongoSetting;
        private readonly ILogger<ChatRepository> _logger;

        public ChatRepository(ILogger<ChatRepository> logger, IOptions<MongoConfigSetting> mongoSetting)
        {
            _mongoSetting = mongoSetting;
            _logger = logger;
            _logger.LogInformation($"HostName: {_mongoSetting.Value.HostName}\nDB: {_mongoSetting.Value.DatabaseName}\nCols: {MongoConfigSetting.Collections["Chat"]}");
            IMongoClient mongoClient = new MongoClient(_mongoSetting.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(_mongoSetting.Value.DatabaseName);
            _chatCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["Chat"]);
        }
        public async Task<Response> AddChat(ChatModel chat)
        {
            string strChat = JsonConvert.SerializeObject(chat);
            BsonDocument document = BsonDocument.Parse(
                strChat
            );

            await _chatCols.InsertOneAsync(document);
            _logger.LogInformation("New Chat added!");
            return new Response
            {
                Message = "New Chat added!",
                Data = chat,
                StatusCode = StatusCodes.Status201Created
            };
        }
        public async Task<ChatModel> FindChat(string groupId)
        {
            // find the existing user
            var existingChat = await _chatCols
                .Find(x => x["group"]["group_id"] == groupId)
                .As<ChatModel>()
                .FirstOrDefaultAsync();

            if (existingChat == null)
            {
                // _logger.LogError("Chat existed!");
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    $"Chat with group id {groupId} not found",
                    new List<Error>()
                );
            }
            
            return existingChat;
        }
    }
}