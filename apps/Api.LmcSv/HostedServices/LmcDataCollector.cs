using Api.LmcLib.Interfaces;
using Api.LmcLib.Stores;
using Simple.RabbitMQ;
using Api.LmcLib.Models;
using Newtonsoft.Json;
using Api.LmcLib.Exceptions;

namespace Api.LmcSv.HostedServices
{
    public class LmcDataCollector : IHostedService
    {
        private readonly IMessageSubscriber _subscriber;
        private readonly ILogger<LmcDataCollector> _logger;
        private readonly IServiceProvider _serviceProvider;
        public LmcDataCollector(IMessageSubscriber subscriber,ILogger<LmcDataCollector> logger,IServiceProvider serviceProvider)
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
                IUserChatRepository _userChatRepo = scope.ServiceProvider.GetRequiredService<IUserChatRepository>();
                IChatConsumer _chatConsumer = scope.ServiceProvider.GetRequiredService<IChatConsumer>();
                IUserRepository _userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
                IMessageConsumer _msgConsumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
                IPlatformRepository _platformRepo = scope.ServiceProvider.GetRequiredService<IPlatformRepository>();
                IUserConsumer _userConsumer = scope.ServiceProvider.GetRequiredService<IUserConsumer>();
                IDictionary<string, string> msgRoutingKeys = RoutingKeys.Message;
                if (routingKey == RoutingKeys.Auth["update"])
                {
                    await _authConsumer.ConsumeAuthUpdate(message);
                    await _userConsumer.ConsumeAuthUpdate(message);

                    LineAuthStateModel authModel = JsonConvert
                        .DeserializeObject<LineAuthStateModel>(message)!;
                    PlatformModel platformModel = new PlatformModel();
                    try
                    {
                        platformModel = await _platformRepo.Find(authModel.PlatformId!);

                        if (platformModel != null)
                        {
                            _logger.LogError("Platform exist!");
                            return true;
                        }
                    }
                    catch (ErrorResponseException ex)
                    {
                        _logger.LogInformation(ex.Description);
                        platformModel = new PlatformModel();
                    }

                    platformModel = new PlatformModel
                    {
                        PlatformId = authModel.PlatformId,
                        AccessToken = authModel.AccessToken,
                        SecretKey = authModel.SecretKey
                    };

                    await _platformRepo.AddPlatform(platformModel);
                }
                else if (routingKey == RoutingKeys.Platform["verify"])
                {
                    Console.WriteLine("jp");
                    await _userConsumer.ConsumePlatformVerify(message);
                    await _authConsumer.ConsumePlatformVerify(message);
                    
                }
                else if (routingKey == RoutingKeys.UserChat["create"])
                {
                    UserChatModel userChatModel = JsonConvert.DeserializeObject<UserChatModel>(message)!;
                    await _userChatRepo.AddUserChat(userChatModel);
                }
                else if (routingKey == RoutingKeys.UserChat["verify"])
                {
                    await _chatConsumer.ConsumeUserChatVerify(message);
                }
                else if (routingKey == RoutingKeys.User["create"])
                {
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
                    await _msgConsumer.ConsumeMessageCreate(message);
                }
                else if (routingKey == msgRoutingKeys["create"])
                {
                    await _userConsumer.ConsumeMessageCreate(message);
                }
                else if (routingKey == RoutingKeys.Message["verify"])
                {
                    await _userConsumer.ConsumeMessageVerify(message);
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