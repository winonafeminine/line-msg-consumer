// using Api.LmcLib.Interfaces;
// using Api.LmcLib.Stores;
// using Api.LmcLib.Models;
// using Newtonsoft.Json;
// using Simple.RabbitMQ;

// namespace Api.LmcSv.HostedServices
// {
//     public class ChatDataCollector : IHostedService
//     {
//         private readonly ILogger<ChatDataCollector> _logger;
//         private readonly IMessageSubscriber _subscriber;
//         private readonly IServiceProvider _serviceProvider;
//         public ChatDataCollector(ILogger<ChatDataCollector> logger, IMessageSubscriber subscriber, IServiceProvider serviceProvider)
//         {
//             _logger = logger;
//             _subscriber = subscriber;
//             _serviceProvider = serviceProvider;
//         }
//         public Task StartAsync(CancellationToken cancellationToken)
//         {
//             _subscriber.SubscribeAsync(ProcessMessage);
//             return Task.CompletedTask;
//         }

//         public async Task<bool> ProcessMessage(string message, IDictionary<string, object> headers, string routingKey)
//         {
//             // await Task.Yield();
//             using (var scope = _serviceProvider.CreateAsyncScope())
//             {
//                 IUserChatRepository _userChatRepo = scope.ServiceProvider.GetRequiredService<IUserChatRepository>();
//                 IChatConsumer _chatConsumer = scope.ServiceProvider.GetRequiredService<IChatConsumer>();
//                 if (routingKey == RoutingKeys.UserChat["create"])
//                 {
//                     // _logger.LogInformation($"Routing key: {routingKey}\nMessage: {message}");
//                     // await _chatMsgConsumer.ConsumeMessageCreate(message);
//                     UserChatModel userChatModel = JsonConvert.DeserializeObject<UserChatModel>(message)!;
//                     await _userChatRepo.AddUserChat(userChatModel);
//                 }
//                 else if (routingKey == RoutingKeys.UserChat["verify"])
//                 {
//                     await _chatConsumer.ConsumeUserChatVerify(message);
//                 }
//             }
//             return true;
//         }

//         public Task StopAsync(CancellationToken cancellationToken)
//         {
//             _logger.LogInformation("Subscriber disposed!");
//             return Task.CompletedTask;
//         }
//     }
// }