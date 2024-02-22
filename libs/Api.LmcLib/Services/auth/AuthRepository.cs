using Api.LmcLib.DTOs;
using Api.LmcLib.Interfaces;
using Api.LmcLib.Models;
using Api.LmcLib.Exceptions;
using Api.LmcLib.Setttings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Api.LmcLib.Services
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ILogger<AuthRepository> _logger;
        private readonly IMongoCollection<BsonDocument> _authCols;
        private readonly IOptions<MongoConfigSetting> _mongoSetting;
        public AuthRepository(ILogger<AuthRepository> logger, IOptions<MongoConfigSetting> mongoSetting)
        {
            _mongoSetting = mongoSetting;
            IMongoClient mongoClient = new MongoClient(_mongoSetting!.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(_mongoSetting!.Value.DatabaseName);
            _authCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["Auth"]);
            _logger = logger;
        }
        public async Task<LineAuthStateModel> CreateAuth(AuthDto authDto)
        {
            string state = ObjectId.GenerateNewId().ToString();

            LineAuthStateModel authModel = new LineAuthStateModel
            {
                State = state,
                AppRedirectUri = authDto.AppRedirectUri
            };

            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(authModel)
            );

            await _authCols.InsertOneAsync(document);

            return authModel;
        }

        public async Task<LineAuthStateModel> FindAuth(string state)
        {
            LineAuthStateModel authModel = await _authCols
                .Find(x=>x["state"] == state)
                .As<LineAuthStateModel>()
                .FirstOrDefaultAsync();
            
            if(authModel == null)
            {
                throw new ErrorResponseException(
                    StatusCodes.Status200OK,
                    $"Auth with state {state} not found",
                    new List<Error>()
                );
            }
            return authModel;
        }

        public IEnumerable<LineAuthStateModel> GetAuths()
        {
            return _authCols.Find(x=>true)
                .As<LineAuthStateModel>()
                .ToEnumerable();
        }

        public async Task<Response> RemoveStateByPlatformId(string platformId)
        {
            LineAuthStateModel authModel = await _authCols
                .Find(x=>x["platform_id"] == platformId)
                .As<LineAuthStateModel>()
                .FirstOrDefaultAsync();

            if(authModel == null)
            {
                throw new ErrorResponseException(
                    StatusCodes.Status404NotFound,
                    $"Auth with platform_id {platformId} not found",
                    new List<Error>()
                );
            }

            await _authCols.DeleteOneAsync(x=>x["platform_id"] == platformId);
            string resMsg = "Successfully remove the auth";
            _logger.LogInformation(resMsg);
            return new Response{
                Message=resMsg,
            };
        }
    }
}