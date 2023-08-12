using Api.PlatformLib.DTOs;
using Api.ReferenceLib.Models;

namespace Api.PlatformLib.Interfaces
{
    public interface IPlatformService
    {
        public Task<Response> UpdatePlatform(string action, string platformId, PlatformDto platformDto);
    }
}