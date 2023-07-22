using Simple.RabbitMQ;

namespace Api.MessageSv.HostedServices
{
    public class MessageDataCollector : IHostedService
    {
        private readonly ILogger<MessageDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        public MessageDataCollector(ILogger<MessageDataCollector> logger, IMessageSubscriber subscriber)
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
            _subscriber.Dispose();
            _logger.LogInformation("Subscriber disposed!");
            return Task.CompletedTask;
        }
    }
}