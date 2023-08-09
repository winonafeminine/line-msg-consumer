using Api.CommonLib.Interfaces;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;

namespace Api.CommonLib.Consumers
{
    public class AuthConsumer : IAuthConsumer
    {
        private readonly IPlatformRepository _platformRepo;
        public AuthConsumer(IPlatformRepository platformRepo)
        {
            _platformRepo = platformRepo;
        }
        public async Task VerifyPlatform(PlatformModel platformModel)
        {
            await _platformRepo.ReplacePlatform(platformModel);
        }
    }
}