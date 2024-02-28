
using Api.LmcLib.Models;

namespace Api.LmcLib.Interfaces
{
    public interface IAuthValidateJwtToken
    {
        Task<JwtPayloadData> ValidateJwtToken(string token);
    }
}
