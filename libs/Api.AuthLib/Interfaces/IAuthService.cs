using Api.AuthLib.DTOs;
using Api.ReferenceLib.Models;

namespace Api.AuthLib.Interfaces
{
    public interface IAuthService
    {
        public Task<Response> CreateLineAuthState(AuthDto auth);
        public Task<Response> UpdateLineAuthState(string state, AuthDto auth);
        public Response GetLineAuthStates();
        public Task<Response> RemoveStateByPlatformId(string platformId);
    }
}