using Api.LmcLib.Models;

namespace Api.LmcLib.Interfaces
{
    public interface IUserChatRepository
    {
        public Task<Response> AddUserChat(UserChatModel userChatModel);
        public Task<UserChatModel> FindUserChatByGroupId(string groupId);
        public Task<UserChatModel> FindUserChat(string groupId, string groupUserId);
    }
}