using System.Security.Claims;
using Api.LmcLib.Models;

namespace Api.LmcLib.Interfaces
{
    public interface IJwtToken
    {
        public string GenerateJwtToken(string secretKey, string issuer, string refId, DateTime? expires);
        public ClaimsPrincipal ValidateJwtToken(string token, string secretKey);
        public JwtPayloadData GetJwtPayloadData(string token);
    }
}