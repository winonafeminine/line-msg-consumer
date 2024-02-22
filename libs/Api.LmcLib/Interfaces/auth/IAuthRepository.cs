using Api.LmcLib.DTOs;
using Api.LmcLib.Models;

namespace Api.LmcLib.Interfaces
{
    public interface IAuthRepository
    {
        public Task<LineAuthStateModel> CreateAuth(AuthDto authDto);
        public IEnumerable<LineAuthStateModel> GetAuths();
        public Task<LineAuthStateModel> FindAuth(string state);
        public Task<Response> RemoveStateByPlatformId(string platformId);
    }
}