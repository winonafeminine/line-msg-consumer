using Api.PlatformLib.DTOs;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.PlatformLib.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly ILogger<PlatformService> _logger;
        private readonly IPlatformRepository _platformRepo;
        private readonly IMessagePublisher _publisher;
        public PlatformService(ILogger<PlatformService> logger, IPlatformRepository platformRepo, IMessagePublisher publisher)
        {
            _logger = logger;
            _platformRepo = platformRepo;
            _publisher = publisher;
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

                // publish the replaced platform
                string routingKey = RoutingKeys.Platform["verify"];
                string strPlatform = JsonConvert.SerializeObject(platformModel);
                string groupUserId = platformDto.GroupUserId!;
                platformDto = JsonConvert.DeserializeObject<PlatformDto>(strPlatform)!;
                platformDto.PlatformId = platformModel.PlatformId;
                platformDto.GroupUserId = groupUserId;
                strPlatform = JsonConvert.SerializeObject(platformDto);
                try
                {
                    _publisher.Publish(strPlatform, routingKey, null);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                finally
                {
                    _publisher.Dispose();
                }
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