using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
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

        public async Task<Response> Find(string platformId)
        {
            // find the existing user
            var existingPlatforms = await _platformCols.Find<BsonDocument>(x => x["_id"] == platformId)
                .ToListAsync();

            if (existingPlatforms.Any())
            {
                // _logger.LogError("Platform existed!");
                return new Response
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Platform existed!"
                };
            }

            return new Response
            {
                StatusCode = StatusCodes.Status404NotFound,
                Message = "Platform not found"
            };
        }
    }
}