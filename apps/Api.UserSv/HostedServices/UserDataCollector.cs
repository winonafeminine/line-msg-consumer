using Simple.RabbitMQ;

namespace Api.UserSv.HostedServices
{
    public class UserDataCollector : IHostedService
    {
        private readonly ILogger<UserDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;

        public UserDataCollector(ILogger<UserDataCollector> logger, IMessageSubscriber subscriber)
        {
            _logger = logger;
            _subscriber = subscriber;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _subscriber.SubscribeAsync(ProcessMessage);
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            await Task.Yield();
            _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
            return true;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subscriber disposed!");
            return Task.CompletedTask;
        }
    }
}