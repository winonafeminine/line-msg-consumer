using Api.AuthLib.Models;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
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

                PlatformModel platformModel = new PlatformModel();
                try{
                    platformModel = await _platformRepo.Find(authModel.PlatformId!);

                    if(platformModel != null)
                    {
                        _logger.LogError("Platform exist!");
                        return true;
                    }
                }catch (ErrorResponseException ex){
                    _logger.LogInformation(ex.Description);
                    platformModel = new PlatformModel();
                }

                platformModel = new PlatformModel{
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