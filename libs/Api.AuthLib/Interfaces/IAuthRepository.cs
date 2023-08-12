using Api.AuthLib.DTOs;
using Api.ReferenceLib.Models;

namespace Api.AuthLib.Interfaces
{
    public interface IAuthRepository
    {
        public Response CreateLineAuthState(AuthDto auth);
        public Task<Response> UpdateLineAuthState(string state, AuthDto auth);
        public Response GetLineAuthStates();
        public Response RemoveStateByPlatformId(string platformId);
    }
}