using Api.CommonLib.Models;

namespace Api.CommonLib.Interfaces
{
    public interface IUserRepository
    {
        public Response AddUser(UserModel user);
        public Task<Response> AddUserAsync(UserModel user);
    }
}