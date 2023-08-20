using Api.CommonLib.Models;
using Api.ReferenceLib.Models;

namespace Api.ChatLib.Interfaces
{
    public interface IChatGrpcClientService
    {
        public Task<ChatModel> GetChat(string groupId);
        public Task<Response> AddChat(ChatModel chat);
    }
}