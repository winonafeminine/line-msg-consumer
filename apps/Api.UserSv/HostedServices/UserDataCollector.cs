using Api.CommonLib.Interfaces;
using Api.ReferenceLib.Stores;
using Simple.RabbitMQ;

namespace Api.UserSv.HostedServices
{
    public class UserDataCollector : IHostedService
    {
        private readonly ILogger<UserDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        private readonly IUserConsumer _userConsumer;

        public UserDataCollector(ILogger<UserDataCollector> logger, IMessageSubscriber subscriber, IUserConsumer userConsumer)
        {
            _logger = logger;
            _subscriber = subscriber;
            _userConsumer = userConsumer;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _subscriber.SubscribeAsync(ProcessMessage);
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            // await Task.Yield();
            // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
            IDictionary<string, string> msgRoutingKeys = RoutingKeys.Message;
            if(routingKey == msgRoutingKeys["create"])
            {
                await _userConsumer.ConsumeMessageCreate(message);
            }
            else if(routingKey == RoutingKeys.Platform["verify"])
            {
                await _userConsumer.ConsumePlatformVerify(message);
            }
            else if(routingKey == RoutingKeys.Auth["update"])
            {
                await _userConsumer.ConsumeAuthUpdate(message);
            }
            else if(routingKey == RoutingKeys.Message["verify"])
            {
                await _userConsumer.ConsumeMessageVerify(message);
            }
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscriber disposed!");
            return Task.CompletedTask;
        }
    }
}