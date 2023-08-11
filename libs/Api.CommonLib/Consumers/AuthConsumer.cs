using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.CommonLib.Interfaces;
using Api.PlatformLib.DTOs;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.CommonLib.Consumers
{
    public class AuthConsumer : IAuthConsumer
    {
        private readonly ILogger<AuthConsumer> _logger;
        private readonly IPlatformRepository _platformRepo;
        private readonly IAuthRepository _authRepo;
        public AuthConsumer(IPlatformRepository platformRepo, ILogger<AuthConsumer> logger, IAuthRepository authRepo)
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
            _authRepo.RemoveStateByPlatformId(platformDto.PlatformId!);
        }
    }
}