using Api.PlatformLib.DTOs;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Models;

namespace Api.PlatformLib.Interfaces
{
    public interface IPlatformRepository
    {
        public Task<PlatformModel> Find(string platformId);
        public Task<Response> AddPlatform(PlatformModel platformModel);
        public Task<Response> ReplacePlatform(PlatformModel platformModel);
    }
}