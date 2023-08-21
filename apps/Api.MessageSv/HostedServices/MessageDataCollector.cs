using Api.CommonLib.Interfaces;
using Api.ReferenceLib.Stores;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.MessageSv.HostedServices
{
    public class MessageDataCollector : IHostedService
    {
        private readonly ILogger<MessageDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        private readonly IServiceProvider _serviceProvider;
        public MessageDataCollector(ILogger<MessageDataCollector> logger, IMessageSubscriber subscriber, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _subscriber = subscriber;
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(ProcessMessage);
            return Task.CompletedTask;
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            // consume the user message when user is created
            using (var scope = _serviceProvider.CreateAsyncScope())
            {
                IUserRepository _userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                IMessageConsumer _msgConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();

                if (routingKey == RoutingKeys.User["create"])
                {
                    // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
                    UserModel userModel = new UserModel();
                    try
                    {
                        userModel = JsonConvert.DeserializeObject<UserModel>(message)!;
                    }
                    catch
                    {
                        _logger.LogInformation("Failed deserializing UserModel");
                    }
                    await _userRepo.AddUser(userModel);
                }
                else if (routingKey == RoutingKeys.Message["create"])
                {
                    // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
                    await _msgConsumer.ConsumeMessageCreate(message);
                }
            }

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