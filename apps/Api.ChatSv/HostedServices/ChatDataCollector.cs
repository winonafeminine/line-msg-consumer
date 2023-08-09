using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Stores;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.ChatSv.HostedServices
{
    public class ChatDataCollector : IHostedService
    {
        private readonly ILogger<ChatDataCollector> _logger;
        private readonly IMessageSubscriber _subscriber;
        private readonly IMessageConsumer _chatMsgConsumer;
        private readonly IUserChatRepository _userChatRepo;

        public ChatDataCollector(ILogger<ChatDataCollector> logger, IMessageSubscriber subscriber, IMessageConsumer chatMsgConsumer, IUserChatRepository userChatRepo)
        {
            _logger = logger;
            _subscriber = subscriber;
            _chatMsgConsumer = chatMsgConsumer;
            _userChatRepo = userChatRepo;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscriber.SubscribeAsync(ProcessMessage);
            return Task.CompletedTask;
        }

        public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
        {
            // await Task.Yield();
            IDictionary<string, string> msgRoutingKeys = RoutingKeys.Message;
            if (routingKey == msgRoutingKeys["create"])
            {
                // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
                await _chatMsgConsumer.ConsumeMessageCreate(message);
            }
            else if (routingKey == RoutingKeys.UserChat["create"])
            {
                // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
                // await _chatMsgConsumer.ConsumeMessageCreate(message);
                UserChatModel userChatModel = JsonConvert.DeserializeObject<UserChatModel>(message)!;
                await _userChatRepo.AddUserChat(userChatModel);
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