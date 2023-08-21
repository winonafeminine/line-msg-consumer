using Api.AuthLib.DTOs;
using Api.AuthLib.Models;
using Api.ReferenceLib.Models;

namespace Api.AuthLib.Interfaces
{
    public interface IAuthRepository
    {
        public Task<LineAuthStateModel> CreateAuth(AuthDto authDto);
        public IEnumerable<LineAuthStateModel> GetAuths();
        public Task<LineAuthStateModel> FindAuth(string state);
        public Task<Response> RemoveStateByPlatformId(string platformId);
    }
}