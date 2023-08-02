using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
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
        public Response AddChat(ChatModel chat)
        {
            string strChat = JsonConvert.SerializeObject(chat);
            BsonDocument document = BsonDocument.Parse(
                strChat
            );

            _chatCols.InsertOne(document);
            _logger.LogInformation("New Chat added!");
            return new Response
            {
                Message = "New Chat added!",
                Data = chat,
                StatusCode = StatusCodes.Status201Created
            };
        }
        public Response FindChat(string groupId)
        {
            // find the existing user
            var existingChats = _chatCols.Find<BsonDocument>(x => x["group"]["group_id"] == groupId)
                .ToList();

            if (existingChats.Any())
            {
                // _logger.LogError("Chat existed!");
                return new Response
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Group existed!"
                };
            }
            
            return new Response
            {
                Message = "Group not found",
                StatusCode = StatusCodes.Status404NotFound
            };
        }
    }
}