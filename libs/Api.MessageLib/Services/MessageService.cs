using System.Text.RegularExpressions;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.MessageLib.Stores;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.MessageLib.Services
{
    public class MessageService : IMessageService
    {
        private readonly ILogger<MessageService> _logger;
        private readonly ILineMessageValidation _lineValidation;
        private readonly IHostEnvironment _env;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly ISpecialKeywordHandler _skHandler;
        private readonly IScopePublisher _publisher;
        public MessageService(ILogger<MessageService> logger, ILineMessageValidation lineValidation,
            IHostEnvironment env, IOptions<MongoConfigSetting> mongoConfig, IOptions<LineChannelSetting> channelSetting, ISpecialKeywordHandler skHandler, IScopePublisher publisher)
        {
            _logger = logger;
            _lineValidation = lineValidation;
            _env = env;
            _channelSetting = channelSetting;
            _skHandler = skHandler;
            _publisher = publisher;
        }

        private string GetValFromJson(string strContent, string keyName, string pattern)
        {
            Match match = Regex.Match(strContent, pattern);

            if (match.Success)
            {
                keyName = match.Groups[1].Value.Replace("\\", "");
                // _logger.LogInformation($"{keyName}: " + keyName);
                return keyName;
            }
            else
            {
                _logger.LogError($"{keyName} not found.");
                throw new ErrorResponseException(
                  StatusCodes.Status404NotFound,
                  $"{keyName} not found",
                  new List<Error>()
                );
            }
        }
        public async Task RetriveLineMessage(object content, string signature, string id)
        {
            // await Task.Yield();

            // only calculate the signature in production mode
            if (!_env.IsDevelopment())
            {
                _lineValidation.Validate(signature, content);
            }

            string strContent = JsonConvert.SerializeObject(content);

            // get the group id value using regex
            string pattern = @"""groupId"":\s*""([^""]+)""";
            string groupId = GetValFromJson(strContent, "groupId", pattern);



            // get the event type
            pattern = @"""type"":\s*""([^""]+)""";
            string eventType = GetValFromJson(strContent, "type", pattern);

            string groupUserId = string.Empty;
            string messageType = string.Empty;

            // check if the event type=message
            if (eventType == LineEventTypes.Message)
            {
                // get the line userId
                pattern = @"""userId"":\s*""([^""]+)""";
                groupUserId = GetValFromJson(strContent, "userId", pattern);
            }

            MessageModel messageModel = new MessageModel
            {
                ClientId = id,
                GroupId = groupId,
                GroupUserId = groupUserId,
                MessageType = MessageTypes.Receive,
                MessageObject = content
            };

            string messageModelStr = JsonConvert.SerializeObject(messageModel);

            // handle the special keywords
            // if true publish to the specific route key
            // catch the error
            try
            {
                _skHandler.HandleGroupVerify(messageModelStr);
            }
            catch
            {
                return;
            }
            
            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(messageModel)
            );

            // await _messageCols.InsertOneAsync(document);

            IDictionary<string, string> msgRoutingKeys = RoutingKeys.Message;
            string routingKey = msgRoutingKeys["create"];
            _publisher.Publish(messageModelStr, routingKey, null);
            
            return;
        }

        public LineChannelSetting GetChannel()
        {
            return new LineChannelSetting
            {
                SecretId = _channelSetting.Value.SecretId,
                ClientId = _channelSetting.Value.ClientId,
                ChannelAccessToken = _channelSetting.Value.ChannelAccessToken,
            };
        }

    }
}