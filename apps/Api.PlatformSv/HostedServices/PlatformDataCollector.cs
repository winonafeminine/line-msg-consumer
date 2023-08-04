using Api.AuthLib.Models;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Stores;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.PlatformSv.HostedServices
{
    public class PlatformDataCollector : IHostedService
    {
        private readonly IMessageSubscriber _subscriber;
        private readonly ILogger<PlatformDataCollector> _logger;
        private readonly IPlatformRepository _platformRepo;
        public PlatformDataCollector(IMessageSubscriber subscriber, ILogger<PlatformDataCollector> logger, IPlatformRepository platformRepo)
        {
            _subscriber = subscriber;
            _logger = logger;
            _platformRepo = platformRepo;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(ProcessMessage);
            return Task.CompletedTask;
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            if(routingKey == RoutingKeys.Auth["update"])
            {
                // _logger.LogInformation(message);
                LineAuthStateModel authModel = JsonConvert
                    .DeserializeObject<LineAuthStateModel>(message)!;

                Response platformResponse = await _platformRepo.Find(authModel.PlatformId!);

                if(platformResponse.StatusCode == StatusCodes.Status409Conflict)
                {
                    _logger.LogWarning(platformResponse.Message);
                    return true;
                }
                PlatformModel platformModel = new PlatformModel{
                    PlatformId=authModel.PlatformId,
                    AccessToken=authModel.AccessToken,
                    SecretKey=authModel.SecretKey
                };

                await _platformRepo.AddPlatform(platformModel);
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriber.Dispose();
            return Task.CompletedTask;
        }
    }
}