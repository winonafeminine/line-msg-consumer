using Api.CommonLib.Models;
using Api.ReferenceLib.Models;
using MongoDB.Bson;

namespace Api.ReferenceLib.Interfaces
{
    public interface IChatRepository
    {
        public Task<Response> AddChat(ChatModel chat);
        public Task<ChatModel> FindChat(string groupId);
        public IEnumerable<T> FindChats<T>(List<BsonDocument> pipeline);
    }
}