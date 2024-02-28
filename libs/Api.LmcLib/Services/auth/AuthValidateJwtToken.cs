using System.IdentityModel.Tokens.Jwt;
using Api.LmcLib.Interfaces;
using Api.LmcLib.Models;
using Newtonsoft.Json;

namespace Api.LmcLib.Services
{
    public class AuthValidateJwtToken : IAuthValidateJwtToken
    {
        public  async Task<JwtPayloadData> ValidateJwtToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var payloadData = new JwtPayloadData
                    {
                        PlatformId = jsonToken.Claims.FirstOrDefault(c => c.Type == "platform_id")?.Value,
                        UserId = jsonToken.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value,
                        NotValidBefore = jsonToken.Claims.FirstOrDefault(c => c.Type == "nbf")?.Value,
                        Expires = jsonToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value,
                        IssuedAt = jsonToken.Claims.FirstOrDefault(c => c.Type == "iat")?.Value,
                        Issuer = jsonToken.Claims.FirstOrDefault(c => c.Type == "iss")?.Value
                    };
                    Console.WriteLine(JsonConvert.SerializeObject(payloadData));
                    return payloadData;
                }
            }
            catch (Exception ex)
            {
                // Handle validation errors or token format issues here
                // You might want to log the error or throw a custom exception
                Console.WriteLine($"Error validating JWT token: {ex.Message}");
            }

            // Return null or throw an exception if the token is not valid
            return null!;
        }
    }

}
