using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Stores;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.MessageSv.HostedServices
{
    public class MessageDataCollector : IHostedService
    {
        private readonly ILogger<MessageDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        private readonly IUserRepository _userRepo;
        public MessageDataCollector(ILogger<MessageDataCollector> logger, IMessageSubscriber subscriber, IUserRepository userRepo)
        {
            _logger = logger;
            _subscriber = subscriber;
            _userRepo = userRepo;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Yield();
            _subscriber.SubscribeAsync(ProcessMessage);
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            await Task.Yield();

            // consume the user message when user is created
            if(routingKey == RoutingKeys.User["create"])
            {
                // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
                UserModel userModel = new UserModel();
                try{
                    userModel = JsonConvert.DeserializeObject<UserModel>(message)!;
                }catch{
                    _logger.LogInformation("Failed deserializing UserModel");
                }
                _userRepo.AddUser(userModel);
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