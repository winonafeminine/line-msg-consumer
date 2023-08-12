// using System.Text;
// using Api.CommonLib.Interfaces;
// using Api.CommonLib.Setttings;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Extensions.Options;
// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;

// namespace Api.CommonLib.RPCs
// {
//     public class CommonRpcClient : ICommonRpcClient
//     {
//         private readonly IConnection _connection;
//         private readonly IModel _channel;
//         private readonly IOptions<RabbitmqConfigSetting> _rabbitmqConfig;

//         public CommonRpcClient(IConfiguration configuration, IOptions<RabbitmqConfigSetting> rabbitmqConfig)
//         {
//             _rabbitmqConfig = rabbitmqConfig;
//             var factory = new ConnectionFactory
//             {
//                 Uri = new Uri(_rabbitmqConfig.Value.HostName!)
//             };

//             _connection = factory.CreateConnection();
//             _channel = _connection.CreateModel();
//         }

//         public void Dispose()
//         {
//             _connection.Close();
//         }

//         public string SendRPCRequest(string request, string queueName)
//         {
//             // Declare a callback queue to receive the RPC response
//             var callbackQueueName = _channel.QueueDeclare().QueueName;
//             var correlationId = Guid.NewGuid().ToString();

//             // Set up the properties for the RPC request
//             var props = _channel.CreateBasicProperties();
//             props.CorrelationId = correlationId;
//             props.ReplyTo = callbackQueueName;

//             // Convert the message to a byte array and publish it
//             var messageBytes = Encoding.UTF8.GetBytes(request);
//             _channel.BasicPublish(
//                 exchange: "",
//                 routingKey: queueName,
//                 basicProperties: props,
//                 body: messageBytes
//             );

//             // Set up a consumer to receive the RPC response
//             var responseConsumer = new EventingBasicConsumer(_channel);
//             string? response = null;
//             responseConsumer.Received += (model, ea) =>
//             {
//                 if (ea.BasicProperties.CorrelationId == correlationId)
//                 {
//                     var responseBytes = ea.Body.ToArray();
//                     response = Encoding.UTF8.GetString(responseBytes);
//                 }
//             };

//             _channel.BasicConsume(
//                 queue: callbackQueueName,
//                 autoAck: true,
//                 consumer: responseConsumer
//             );

//             // Wait for the response and return it
//             while (response == null)
//             {
//                 // You can add a timeout logic here to prevent an infinite loop
//             }

//             return response;
//         }
//     }
// }