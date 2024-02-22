using Api.LmcLib.DTOs;
using Api.LmcLib.Models;

namespace Api.LmcLib.Interfaces
{
    public interface IPlatformService
    {
        public Task<Response> UpdatePlatform(string action, string platformId, PlatformDto platformDto);
    }
}