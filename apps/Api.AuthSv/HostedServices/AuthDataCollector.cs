using Api.AuthLib.Models;
using Api.CommonLib.Interfaces;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Stores;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.AuthSv.HostedServices
{
    public class AuthDataCollector : IHostedService
    {
        private readonly IMessageSubscriber _subscriber;
        private readonly ILogger<AuthDataCollector> _logger;
        private readonly IPlatformRepository _platformRepo;
        private readonly IAuthConsumer _authConsumer;
        public AuthDataCollector(IMessageSubscriber subscriber, ILogger<AuthDataCollector> logger, IPlatformRepository platformRepo, IAuthConsumer authConsumer)
        {
            _subscriber = subscriber;
            _logger = logger;
            _platformRepo = platformRepo;
            _authConsumer = authConsumer;
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
            }else if(routingKey == RoutingKeys.Platform["verify"])
            {
                PlatformModel platformModel = JsonConvert.DeserializeObject<PlatformModel>(message)!;
                await _authConsumer.VerifyPlatform(platformModel);
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