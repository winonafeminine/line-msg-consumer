using Api.LmcLib.Exceptions;
using Api.LmcLib.Models;
using Api.LmcLib.Setttings;
using Api.LmcLib.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.UserLib.Services
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
        public async Task<Response> AddUser(UserModel user)
        {
            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(user)
            );

            await _userCols.InsertOneAsync(document);
            _logger.LogInformation("New user added!");
            return new Response
            {
                Message = "New user added!",
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

        public async Task<UserModel> FindUser(string userId)
        {
            // find the existing user
            var existingUser = await _userCols
                .Find<BsonDocument>(x => x["group_user_id"] == userId)
                .As<UserModel>()
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                // _logger.LogError("user existed!");
                return existingUser;
            }
            string resMessage = $"User with id {userId} not found";
            _logger.LogWarning(resMessage);
            throw new ErrorResponseException(
                StatusCodes.Status404NotFound,
                resMessage,
                new List<Error>()
            );
        }

        public async Task<Response> ReplaceUser(UserModel userModel)
        {
            string resMessage = string.Empty;
            string strDoc = JsonConvert.SerializeObject(userModel);
            BsonDocument document = BsonDocument.Parse(
                strDoc
            );

            var filter = Builders<BsonDocument>.Filter.Eq(x => x["_id"], userModel.UserId);
            var updateResult = await _userCols.ReplaceOneAsync(filter, document);
            resMessage = $"Successfully replace the user";
            _logger.LogInformation(resMessage);

            return new Response
            {
                StatusCode = StatusCodes.Status200OK,
                Message = resMessage,
                Data = userModel
            };
        }
    }
}