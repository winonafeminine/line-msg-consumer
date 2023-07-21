using System.Text.Json;
using Api.MessageLib.Interfaces;
using Microsoft.Extensions.Logging;

namespace Api.MessageLib.Services
{
    public class LineMessagingService : ILineMessaging
    {
        private readonly ILogger<LineMessagingService> _logger;
        private readonly ILineMessageValidation _lineValidation;
        public LineMessagingService(ILogger<LineMessagingService> logger, ILineMessageValidation lineValidation)
        {
            _logger = logger;
            _lineValidation = lineValidation;
        }
        public async Task RetriveLineMessage(object content, string signature, string id)
        {
            await Task.Yield();
            _logger.LogInformation($"Client ID: {id}");
            _logger.LogInformation(JsonSerializer.Serialize(content));
            _lineValidation.Validate(signature, content);
            return;
        }
    }
}