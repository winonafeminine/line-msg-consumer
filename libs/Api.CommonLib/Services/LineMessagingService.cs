using System.Text.RegularExpressions;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Stores;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Api.UserLib.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.CommonLib.Services
{
    public class LineMessagingService : ILineMessaging
    {
        private readonly ILogger<LineMessagingService> _logger;
        private readonly ILineMessageValidation _lineValidation;
        private readonly IMessagePublisher _publisher;
        private readonly IHostEnvironment _env;
        private readonly IMongoCollection<BsonDocument> _messageCols;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IUserRepository _userRepo;
        public LineMessagingService(ILogger<LineMessagingService> logger, ILineMessageValidation lineValidation,
            IMessagePublisher publisher, IHostEnvironment env, IOptions<MongoConfigSetting> mongoConfig, IOptions<LineChannelSetting> channelSetting, IUserRepository userRepo)
        {
            _logger = logger;
            _lineValidation = lineValidation;
            _publisher = publisher;
            _env = env;

            IMongoClient mongoClient = new MongoClient(mongoConfig.Value.HostName);
            IMongoDatabase mongodb = mongoClient.GetDatabase(mongoConfig.Value.DatabaseName);
            _messageCols = mongodb.GetCollection<BsonDocument>(MongoConfigSetting.Collections["Message"]);
            _channelSetting = channelSetting;
            _userRepo = userRepo;
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

            BsonDocument document = BsonDocument.Parse(
                JsonConvert.SerializeObject(messageModel)
            );

            await _messageCols.InsertOneAsync(document);

            try
            {
                string messageModelStr = JsonConvert.SerializeObject(messageModel);
                IDictionary<string, string> msgRoutingKeys = RoutingKeys.Message;
                string routingKey = msgRoutingKeys["create"];
                _publisher.Publish(messageModelStr, routingKey, null);
                // _logger.LogInformation($"Message pulished\nRouting key: {routingKey}");
            }
            finally
            {
                _logger.LogInformation($"Message publisher disposed");
                _publisher.Dispose();
            }
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

        public Response AddUser(UserModel user)
        {
            return _userRepo.AddUser(user);
        }
    }
}