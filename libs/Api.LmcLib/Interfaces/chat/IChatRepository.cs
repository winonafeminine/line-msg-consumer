using Api.LmcLib.Models;
using MongoDB.Bson;

namespace Api.LmcLib.Interfaces
{
    public interface IChatRepository
    {
        public Task<Response> AddChat(ChatModel chat);
        public Task<ChatModel> FindChat(string groupId);
        public IEnumerable<T> FindChats<T>(List<BsonDocument> pipeline);
    }
}