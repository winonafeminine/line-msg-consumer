using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.AuthLib.Stores;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Api.AuthLib.Services
{
    public class JwtTokenService : IJwtToken
    {
        public string GenerateJwtToken(string secretKey, string issuer, string refId, DateTime? expires)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenClaim = new Claim("", "");

            if (issuer == JwtIssuers.Platform)
            {
                tokenClaim = new Claim("platform_id", refId);
            }
            else if (issuer == JwtIssuers.user)
            {
                tokenClaim = new Claim("user_id", refId);
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Add any custom claims if needed
                    tokenClaim
                }),
                Issuer = issuer,
                Expires = expires, // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JwtPayloadData GetJwtPayloadData(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            string strPayload = jwtToken.Payload.SerializeToJson();
            return JsonConvert.DeserializeObject<JwtPayloadData>(strPayload)!;
        }

        public ClaimsPrincipal ValidateJwtToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero // Optionally, you can adjust the allowed clock skew
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            return principal;
        }
    }
}