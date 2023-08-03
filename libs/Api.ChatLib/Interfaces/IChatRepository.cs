using Api.CommonLib.Models;
using Api.ReferenceLib.Models;

namespace Api.ReferenceLib.Interfaces
{
    public interface IChatRepository
    {
        public Response AddChat(ChatModel chat);
        public Response FindChat(string groupId);
    }
}