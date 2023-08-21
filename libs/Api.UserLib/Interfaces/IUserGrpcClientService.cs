using Api.ReferenceLib.Models;
using Api.UserLib.Models;

namespace Api.UserLib.Interfaces
{
    public interface IUserGrpcClientService
    {
        public Task<Response> AddUser(UserModel model);
    }
}