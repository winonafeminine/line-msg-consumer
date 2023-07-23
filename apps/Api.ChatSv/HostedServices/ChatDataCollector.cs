using Api.CommonLib.Interfaces;
using Api.CommonLib.Stores;
using Simple.RabbitMQ;

namespace Api.ChatSv.HostedServices
{
    public class ChatDataCollector : IHostedService
    {
        private readonly ILogger<ChatDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        private readonly IMessageConsumer _chatMsgConsumer;

        public ChatDataCollector(ILogger<ChatDataCollector> logger, IMessageSubscriber subscriber, IMessageConsumer chatMsgConsumer)
        {
            _logger = logger;
            _subscriber = subscriber;
            _chatMsgConsumer = chatMsgConsumer;
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
            if (routingKey == msgRoutingKeys["create"])
            {
                await _chatMsgConsumer.ConsumeMessageCreate(message);
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