using Api.PlatformLib.Models;
using Api.ReferenceLib.Models;

namespace Api.PlatformLib.Interfaces
{
    public interface IPlatformRepository
    {
        public Task<Response> Find(string platformId);
        public Task<Response> AddPlatform(PlatformModel platformModel);
    }
}