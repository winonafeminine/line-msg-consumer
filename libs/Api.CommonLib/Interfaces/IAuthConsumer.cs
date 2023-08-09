using Api.PlatformLib.Models;

namespace Api.CommonLib.Interfaces
{
    public interface IAuthConsumer
    {
        public Task VerifyPlatform(PlatformModel platformModel);
    }
}