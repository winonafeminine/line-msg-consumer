using Api.CommonLib.Interfaces;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Api.UserLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.CommonLib.Services
{
    public class UserChatRepository : IUserChatRepository
    {
        private readonly ILogger<UserChatRepository> _logger;
        private readonly IMongoCollection<BsonDocument> _userChatCols;
        private readonly IOptions<MongoConfigSetting> _mongoSetting;

        public UserChatRepository(ILogger<UserChatRepository> logger, IOptions<MongoConfigSetting> mongoSetting)
        {
            _mongoSetting = mongoSetting;
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(_mongoSetting.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(_mongoSetting.Value.DatabaseName);
            _userChatCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["UserChat"]);
        }
        public async Task<Response> AddUserChat(UserChatModel userChatModel)
        {
            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(userChatModel)
            );

            await _userChatCols.InsertOneAsync(document);
            string message = "New User chat added!";
            _logger.LogInformation(message);
            return new Response
            {
                Message = message,
                Data = userChatModel,
                StatusCode = StatusCodes.Status201Created
            };
        }
    }
}