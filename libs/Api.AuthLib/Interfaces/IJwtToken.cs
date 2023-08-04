using System.Security.Claims;

namespace Api.AuthLib.Interfaces
{
    public interface IJwtToken
    {
        public string GenerateJwtToken(string secretKey, string issuer, string refId);
        public ClaimsPrincipal  ValidateJwtToken(string token, string secretKey);
    }
}