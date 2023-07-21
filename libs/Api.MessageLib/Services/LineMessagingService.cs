using System.Text.Json;
using Api.MessageLib.Interfaces;
using Microsoft.Extensions.Logging;

namespace Api.MessageLib.Services
{
    public class LineMessagingService : ILineMessaging
    {
        private readonly ILogger<LineMessagingService> _logger;
        public LineMessagingService(ILogger<LineMessagingService> logger)
        {
            _logger = logger;
        }
        public async Task RetriveLineMessage(object content, string id)
        {
            await Task.Yield();
            _logger.LogInformation($"Client ID: {id}");
            _logger.LogInformation(JsonSerializer.Serialize(content));
            return;
        }
    }
}