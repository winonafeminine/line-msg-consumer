using System.Text;
using Api.CommonLib.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Api.MessageSv.HostedServices
{
    public class MessageRpcServer : IHostedService
    {
        private readonly ILogger<MessageRpcServer> _logger;
        private readonly IOptions<RabbitmqConfigModel> _rabbitmqConfig;
        public MessageRpcServer(ILogger<MessageRpcServer> logger, IOptions<RabbitmqConfigModel> rabbitmqConfig)
        {
            _logger = logger;
            _rabbitmqConfig = rabbitmqConfig;
        }
        public void GetFibonacy()
        {
            var factory = new ConnectionFactory { Uri=new Uri(_rabbitmqConfig.Value.HostName!) };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "rpc_queue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: "rpc_queue",
                                 autoAck: false,
                                 consumer: consumer);
            _logger.LogInformation(" [x] Awaiting RPC requests");

            consumer.Received += (model, ea) =>
            {
                string response = string.Empty;

                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    int n = int.Parse(message);
                    _logger.LogInformation($" [.] Fib({message})");
                    response = Fib(n).ToString();
                }
                catch (Exception e)
                {
                    _logger.LogInformation($" [.] {e.Message}");
                    response = string.Empty;
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: string.Empty,
                                         routingKey: props.ReplyTo,
                                         basicProperties: replyProps,
                                         body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            _logger.LogInformation(" Press [enter] to exit.");

            // Assumes only valid positive integer input.
            // Don't expect this one to work for big numbers, and it's probably the slowest recursive implementation possible.
            static int Fib(int n)
            {
                if (n is 0 or 1)
                {
                    return n;
                }

                return Fib(n - 1) + Fib(n - 2);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            GetFibonacy();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}