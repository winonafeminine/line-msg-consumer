using Api.LmcLib.Interfaces;
using Api.LmcLib.Models;
using Api.LmcLib.DTOs;
using Api.LmcLib.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.LmcLib.Consumers
{
    public class AuthConsumer : IAuthConsumer
    {
        private readonly ILogger<AuthConsumer> _logger;
        private readonly IPlatformRepository _platformRepo;
        private readonly IAuthService _authRepo;
        public AuthConsumer(IPlatformRepository platformRepo, ILogger<AuthConsumer> logger, IAuthService authRepo)
        {
            _platformRepo = platformRepo;
            _logger = logger;
            _authRepo = authRepo;
        }

        public async Task ConsumeAuthUpdate(string message)
        {
            // _logger.LogInformation(message);
            LineAuthStateModel authModel = JsonConvert
                .DeserializeObject<LineAuthStateModel>(message)!;

            PlatformModel platformModel = new PlatformModel();
            try
            {
                platformModel = await _platformRepo.Find(authModel.PlatformId!);

                if (platformModel != null)
                {
                    _logger.LogError("Platform exist!");
                    return;
                }
            }
            catch (ErrorResponseException ex)
            {
                _logger.LogInformation(ex.Description);
                platformModel = new PlatformModel();
            }

            platformModel = new PlatformModel
            {
                PlatformId = authModel.PlatformId,
                AccessToken = authModel.AccessToken,
                SecretKey = authModel.SecretKey
            };

            await _platformRepo.AddPlatform(platformModel);
        }

        public async Task ConsumePlatformVerify(string message)
        {
            PlatformDto platformDto = JsonConvert.DeserializeObject<PlatformDto>(message)!;
            PlatformModel platformModel = JsonConvert.DeserializeObject<PlatformModel>(message)!;
            platformModel.PlatformId=platformDto.PlatformId;
            await _platformRepo.ReplacePlatform(platformModel);
            await _authRepo.RemoveStateByPlatformId(platformDto.PlatformId!);
        }
    }
}