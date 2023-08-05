using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.PlatformLib.Services
{
    public class PlatformRepository : IPlatformRepository
    {
        private readonly IMongoCollection<BsonDocument> _platformCols;
        private readonly ILogger<PlatformRepository> _logger;
        private readonly IOptions<MongoConfigSetting> _mongoConfig;
        public PlatformRepository(ILogger<PlatformRepository> logger, IOptions<MongoConfigSetting> mongoConfig)
        {
            _mongoConfig = mongoConfig;
            MongoClient mongoClient = new MongoClient(_mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(_mongoConfig.Value.DatabaseName);
            _platformCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["Platform"]);
            _logger = logger;
        }
        public async Task<Response> AddPlatform(PlatformModel platformModel)
        {
            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(platformModel)
            );

            await _platformCols.InsertOneAsync(document);
            _logger.LogInformation("New Platform added!");
            return new Response
            {
                Message = "New Platform added!",
                Data = platformModel,
                StatusCode = StatusCodes.Status201Created
            };
        }

        public async Task<PlatformModel> Find(string platformId)
        {
            // find the existing user
            var existingPlatform = await _platformCols
                .Find<BsonDocument>(x => x["_id"] == platformId)
                .As<PlatformModel>()
                .FirstOrDefaultAsync();

            if (existingPlatform != null)
            {
                // _logger.LogError("Platform existed!");
                return existingPlatform;
            }

            throw new ErrorResponseException(
                StatusCodes.Status404NotFound,
                $"Platform with id {platformId} not found",
                new List<Error>()
            );
        }

        public async Task<Response> ReplacePlatform(PlatformModel platformModel)
        {
            string resMessage = string.Empty;
            string strDoc = JsonConvert.SerializeObject(platformModel);
            BsonDocument document = BsonDocument.Parse(
                strDoc
            );

            var filter = Builders<BsonDocument>.Filter.Eq(x=>x["_id"], platformModel.PlatformId);
            var updateResult = await _platformCols.ReplaceOneAsync(filter, document);
            resMessage = $"Successfully replace the platform";
            _logger.LogInformation(resMessage);
            
            return new Response{
                StatusCode=StatusCodes.Status200OK,
                Message=resMessage
            };
        }
    }
}