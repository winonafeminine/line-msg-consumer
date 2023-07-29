using Api.AuthLib.DTOs;
using Api.CommonLib.Models;

namespace Api.AuthLib.Interfaces
{
    public interface IAuthRepository
    {
        public Response CreateLineAuthState(AuthDto auth);
        public Response GetLineAuthStates();
    }
}