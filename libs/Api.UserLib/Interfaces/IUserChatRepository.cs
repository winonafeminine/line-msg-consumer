using Api.ReferenceLib.Models;
using Api.UserLib.Models;

namespace Api.UserLib.Interfaces
{
    public interface IUserChatRepository
    {
        public Task<Response> AddUserChat(UserChatModel userChatModel);
        public Task<UserChatModel> FindUserChatByGroupId(string groupId);
        public Task<UserChatModel> FindUserChat(string groupId, string groupUserId);
    }
}