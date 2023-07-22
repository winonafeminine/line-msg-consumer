using Api.CommonLib.Stores;
using Api.UserLib.Interfaces;
using Simple.RabbitMQ;

namespace Api.UserSv.HostedServices
{
    public class UserDataCollector : IHostedService
    {
        private readonly ILogger<UserDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        private readonly IUserMessage _userMsg;

        public UserDataCollector(ILogger<UserDataCollector> logger, IMessageSubscriber subscriber, IUserMessage userMsg)
        {
            _logger = logger;
            _subscriber = subscriber;
            _userMsg = userMsg;
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
                await _userMsg.HandleMessageCreate(message);
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