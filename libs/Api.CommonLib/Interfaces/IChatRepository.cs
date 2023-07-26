using Api.CommonLib.Models;

namespace Api.CommonLib.Interfaces
{
    public interface IChatRepository
    {
        public Response AddChat(ChatModel chat);
    }
}