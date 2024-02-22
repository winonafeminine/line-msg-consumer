using Api.LmcLib.Interfaces;
using Api.LmcLib.Stores;
using Api.LmcLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Simple.RabbitMQ;

namespace Api.MessageLib.Services
{
    public class SpecialKeywordHandler : ISpecialKeywordHandler
    {
        private readonly ILogger<SpecialKeywordHandler> _logger;
        private readonly IMessagePublisher _publisher;
        public SpecialKeywordHandler(ILogger<SpecialKeywordHandler> logger, IMessagePublisher publisher)
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

            try
            {
                _publisher.Publish(message, routingKey, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            finally
            {
                _publisher.Dispose();
            }

            throw new ErrorResponseException(
                StatusCodes.Status200OK,
                msgRes,
                new List<Error>()
            );
        }
    }
}