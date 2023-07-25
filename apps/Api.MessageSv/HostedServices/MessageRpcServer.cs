using System.Text;
using Api.CommonLib.DTOs;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Api.CommonLib.Stores;
using Api.MessageLib.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api.MessageSv.HostedServices
{
    public class MessageRpcServer : IHostedService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string _queueName;
        private readonly IOptions<RabbitmqConfigSetting> _rabbitmqConfig;
        private readonly IServiceProvider _serviceProvider;
        public MessageRpcServer(IOptions<RabbitmqConfigSetting> rabbitmqConfig, IServiceProvider serviceProvider)
        {
            _rabbitmqConfig = rabbitmqConfig;
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_rabbitmqConfig.Value.HostName!)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            string messageQueue = RpcQueueNames.Message;
            _queueName = messageQueue; // Match the queue name used in RPCController
            _channel.QueueDeclare(queue: _queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            _channel.BasicQos(0, 1, false);
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            _channel.BasicConsume(queue: _queueName,
                                  autoAck: false,
                                  consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                // Process the RPC request and get the response
                var response = ProcessRequest(message);

                var replyProps = _channel.CreateBasicProperties();
                replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

                var responseBytes = Encoding.UTF8.GetBytes(response);
                _channel.BasicPublish(exchange: "",
                                     routingKey: ea.BasicProperties.ReplyTo,
                                     basicProperties: replyProps,
                                     body: responseBytes);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag,
                                  multiple: false);
            };
            return Task.CompletedTask;
        }

        // Add your custom RPC request processing logic here
        private string ProcessRequest(string request)
        {
            using(var scope = _serviceProvider.CreateScope())
            {
                // Example: Echo the request back as the response
                CommonRpcRequest commonRequest = JsonConvert
                    .DeserializeObject<CommonRpcRequest>(request)!;

                // get the channel details
                if(commonRequest.Action == RpcActions.Message["GetChannel"])
                {
                    ILineMessaging lineMessaging = scope.ServiceProvider
                        .GetRequiredService<ILineMessaging>();
                    LineChannelSetting channelSetting = lineMessaging.GetChannel();
                    return JsonConvert.SerializeObject(channelSetting);
                }

                // create a new user
                else if(commonRequest.Action == RpcActions.Message["CreateUser"])
                {
                    ILineMessaging lineMessaging = scope.ServiceProvider
                        .GetRequiredService<ILineMessaging>();
                    
                    string strUser = JsonConvert.SerializeObject(commonRequest.Body);
                    UserModel user = JsonConvert.DeserializeObject<UserModel>(strUser)!;
                    Response response = lineMessaging.AddUser(user);
                    return JsonConvert.SerializeObject(response);
                }
            }

            return string.Empty;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();
            return Task.CompletedTask;
        }
    }
}