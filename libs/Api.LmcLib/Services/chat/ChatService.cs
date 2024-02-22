using Api.LmcLib.DTOs;
using Api.LmcLib.Interfaces;
using Api.LmcLib.Parameters;
using Api.LmcLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;

namespace Api.LmcLib.Services
{
    public class ChatService : IChatService
    {
        private readonly ILogger<ChatService> _logger;
        private readonly IChatRepository _chatRepo;
        public ChatService(ILogger<ChatService> logger, IChatRepository chatRepo)
        {
            _logger = logger;
            _chatRepo = chatRepo;
        }
        public IEnumerable<ChatDto> GetChats(ChatParam param)
        {
            string sortField = param.GetSortField();
            int sortValue = param.GetSortValue();
            var pipeline = new List<BsonDocument>();
            
            try
            {
                pipeline = new List<BsonDocument>
                {
                    new BsonDocument("$sort", new BsonDocument(sortField, sortValue)),
                    new BsonDocument("$match", new BsonDocument("platform_id", param.PlatformId)),
                    new BsonDocument("$limit", param.Limit),
                    new BsonDocument("$project", new BsonDocument
                        {
                            { "_id", 0 },
                            { "chat_id", "$_id" },
                            { "group", 1 },
                            { "created_date", 1 },
                            { "modified_date", 1 },
                            { "latest_message", 1 },
                            { "platform_id", 1 }
                        })
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    ex.Message,
                    new List<Error>()
                );
            }

            return _chatRepo.FindChats<ChatDto>(pipeline);
        }
    }
}