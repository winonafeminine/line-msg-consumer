using Api.CommonLib.Models;
using Api.ReferenceLib.Models;

namespace Api.ReferenceLib.Interfaces
{
    public interface IChatRepository
    {
        public Task<Response> AddChat(ChatModel chat);
        public Task<ChatModel> FindChat(string groupId);
    }
}