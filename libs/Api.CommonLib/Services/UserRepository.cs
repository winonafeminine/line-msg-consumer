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
    public class UserRepository : IUserRepository
    {
        private readonly ILogger<UserRepository> _logger;
        private readonly IMongoCollection<BsonDocument> _userCols;
        private readonly IOptions<MongoConfigSetting> _mongoSetting;

        public UserRepository(ILogger<UserRepository> logger, IOptions<MongoConfigSetting> mongoSetting)
        {
            _mongoSetting = mongoSetting;
            _logger = logger;
            IMongoClient mongoClient = new MongoClient(_mongoSetting.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(_mongoSetting.Value.DatabaseName);
            _userCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["User"]);
        }

        // use for rpc
        // run synchronously
        public Response AddUser(UserModel user)
        {
            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(user)
            );

            _userCols.InsertOne(document);
            _logger.LogInformation("New User added!");
            return new Response
            {
                Message = "New User added!",
                Data = user,
                StatusCode = StatusCodes.Status201Created
            };
        }

        // use for api
        // run asynchronously
        public async Task<Response> AddUserAsync(UserModel user)
        {
            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(user)
            );

            await _userCols.InsertOneAsync(document);
            _logger.LogInformation("New User added!");
            return new Response
            {
                Message = "New User added!",
                Data = user,
                StatusCode = StatusCodes.Status201Created
            };
        }

        public Response FindUser(string userId)
        {
            // find the existing user
            var existingUsers = _userCols.Find<BsonDocument>(x => x["group_user_id"] == userId)
                .ToList();

            if (existingUsers.Any())
            {
                // _logger.LogError("User existed!");
                return new Response
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "User existed!"
                };
            }

            return new Response
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "User not found"
            };
        }
    }
}