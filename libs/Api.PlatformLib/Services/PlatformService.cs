using Api.PlatformLib.DTOs;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Api.PlatformLib.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly ILogger<PlatformService> _logger;
        private readonly IPlatformRepository _platformRepo;
        public PlatformService(ILogger<PlatformService> logger, IPlatformRepository platformRepo)
        {
            _logger = logger;
            _platformRepo = platformRepo;
        }
        public async Task<Response> UpdatePlatform(string action, string platformId, PlatformDto platformDto)
        {
            string resMessage = string.Empty;

            if (string.IsNullOrEmpty(platformDto.PlatformName))
            {
                resMessage = "platform_name can not be null or empty";
                _logger.LogError(resMessage);
                throw new ErrorResponseException(
                    StatusCodes.Status400BadRequest,
                    resMessage,
                    new List<Error>()
                );
            }

            PlatformModel platformModel = new PlatformModel();
            DateTime modifiedDate = platformModel.ModifiedDate;

            // if not exist throw
            platformModel = await _platformRepo.Find(platformId);

            if (action == RouteActions.Verify)
            {
                platformModel.ModifiedDate = modifiedDate;
                platformModel.IsVerified = true;
                platformModel.PlatformName = platformDto.PlatformName;

                Response response = await _platformRepo.ReplacePlatform(platformModel);
                return response;
            }

            resMessage = "Action is not implemented";
            _logger.LogError(resMessage);
            throw new ErrorResponseException(
                StatusCodes.Status501NotImplemented,
                resMessage,
                new List<Error>()
            );
        }
    }
}