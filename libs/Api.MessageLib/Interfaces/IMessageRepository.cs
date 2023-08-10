using Api.MessageLib.Models;
using Api.ReferenceLib.Models;
using MongoDB.Bson;

namespace Api.MessageLib.Interfaces
{
    public interface IMessageRepository
    {
        public IEnumerable<MessageModel> FindMessages(List<BsonDocument> pipeline);
        public Task<MessageModel> FindMessageByGroupId(string groupId);
        public Task<Response> AddMessage(MessageModel model);
    }
}