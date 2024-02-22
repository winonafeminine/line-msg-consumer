using Api.LmcLib.DTOs;
using Api.LmcLib.Models;

namespace Api.LmcLib.Interfaces
{
    public interface IAuthService
    {
        public Task<Response> CreateLineAuthState(AuthDto auth);
        public Task<Response> UpdateLineAuthState(string state, AuthDto auth);
        public Response GetLineAuthStates();
        public Task<Response> RemoveStateByPlatformId(string platformId);
    }
}