using Api.CommonLib.Models;

namespace Api.CommonLib.Interfaces
{
    public interface IUserChatRepository
    {
        public Task<Response> AddUserChat(UserChatModel userChatModel);
    }
}