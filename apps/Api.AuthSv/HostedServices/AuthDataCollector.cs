using Api.CommonLib.Interfaces;
using Api.ReferenceLib.Stores;
using Simple.RabbitMQ;

namespace Api.AuthSv.HostedServices
{
    public class AuthDataCollector : IHostedService
    {
        private readonly IMessageSubscriber _subscriber;
        private readonly ILogger<AuthDataCollector> _logger;
        private readonly IServiceProvider _serviceProvider;
        public AuthDataCollector(IMessageSubscriber subscriber, ILogger<AuthDataCollector> logger, IServiceProvider serviceProvider)
        {
            _subscriber = subscriber;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(ProcessMessage);
            return Task.CompletedTask;
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                IAuthConsumer _authConsumer = scope.ServiceProvider.GetRequiredService<IAuthConsumer>();

                if (routingKey == RoutingKeys.Auth["update"])
                {
                    await _authConsumer.ConsumeAuthUpdate(message);
                }
                else if (routingKey == RoutingKeys.Platform["verify"])
                {
                    await _authConsumer.ConsumePlatformVerify(message);
                }
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