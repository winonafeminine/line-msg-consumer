using Api.CommonLib.Models;

namespace Api.ChatLib.Interfaces
{
    public interface IChatGrpcClientService
    {
        public Task<ChatModel> GetChat(string groupId);
    }
}