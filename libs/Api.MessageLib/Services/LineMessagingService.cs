using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.MessageLib.Services
{
    public class LineMessagingService : ILineMessaging
    {
        private readonly ILogger<LineMessagingService> _logger;
        private readonly ILineMessageValidation _lineValidation;
        private readonly IMessagePublisher _publisher;
        public LineMessagingService(ILogger<LineMessagingService> logger, ILineMessageValidation lineValidation, IMessagePublisher publisher)
        {
            _logger = logger;
            _lineValidation = lineValidation;
            _publisher = publisher;
        }
        public async Task RetriveLineMessage(object content, string signature, string id)
        {
            await Task.Yield();
            _lineValidation.Validate(signature, content);

            MessageModel messageModel = new MessageModel{
                ClientId=id,
                MessageObject=content
            };
            
            try{
                string messageModelStr = JsonConvert.SerializeObject(messageModel);
                string routingKey = "lmc.message.create";
                _publisher.Publish(messageModelStr, routingKey, null);
                _logger.LogInformation($"Message pulished\nRouting key: {routingKey}");
            }finally{
                _logger.LogInformation($"Message publisher disposed");
                _publisher.Dispose();
            }
            return;
        }
    }
}