using Api.ReferenceLib.Models;
using Api.UserLib.Models;

namespace Api.UserLib.Interfaces
{
    public interface IUserChatGrpcClientService
    {
        public Task<Response> AddUserChat(UserChatModel model);
    }
}