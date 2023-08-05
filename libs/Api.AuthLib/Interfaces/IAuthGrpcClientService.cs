using Api.AuthLib.Models;

namespace Api.AuthLib.Interfaces
{
    public interface IAuthGrpcClientService
    {
        public Task<JwtPayloadData> ValidateJwtToken(string token);
    }
}