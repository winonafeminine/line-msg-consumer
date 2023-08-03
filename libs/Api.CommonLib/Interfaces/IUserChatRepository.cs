using Api.ReferenceLib.Models;
using Api.UserLib.Models;

namespace Api.CommonLib.Interfaces
{
    public interface IUserChatRepository
    {
        public Task<Response> AddUserChat(UserChatModel userChatModel);
    }
}