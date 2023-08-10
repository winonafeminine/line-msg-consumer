using Api.MessageLib.Interfaces;
using Api.MessageLib.Stores;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Simple.RabbitMQ;

namespace Api.MessageLib.Services
{
    public class SpecialKeywordHandler : ISpecialKeywordHandler
    {
        private readonly ILogger<SpecialKeywordHandler> _logger;
        private readonly IScopePublisher _publisher;
        public SpecialKeywordHandler(ILogger<SpecialKeywordHandler> logger, IScopePublisher publisher)
        {
            _logger = logger;
            _publisher = publisher;
        }
        public bool HandleGroupVerify(string message)
        {
            // not contain the key words
            if (!message.Contains(SpecialKeywords.GroupVerify))
            {
                return false;
            }

            string msgRes = "Verifying the group...";
            _logger.LogInformation(msgRes);
            string routingKey = RoutingKeys.Message["verify"];

            _publisher.Publish(message, routingKey, null);

            throw new ErrorResponseException(
                StatusCodes.Status200OK,
                msgRes,
                new List<Error>()
            );
        }
    }
}