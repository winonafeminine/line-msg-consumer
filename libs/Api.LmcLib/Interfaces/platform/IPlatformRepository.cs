using Api.LmcLib.Models;


namespace Api.LmcLib.Interfaces
{
    public interface IPlatformRepository
    {
        public Task<PlatformModel> Find(string platformId);
        public Task<Response> AddPlatform(PlatformModel platformModel);
        public Task<Response> ReplacePlatform(PlatformModel platformModel);
    }
}