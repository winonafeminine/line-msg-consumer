using System.Text.RegularExpressions;
using Api.MessageLib.DTOs;
using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.MessageLib.Parameters;
using Api.MessageLib.Routes;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;

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
        private readonly IMessageRepository _msgRepo;
        public MessageService(ILogger<MessageService> logger, ILineMessageValidation lineValidation,
            IHostEnvironment env, IOptions<MongoConfigSetting> mongoConfig, IOptions<LineChannelSetting> channelSetting, ISpecialKeywordHandler skHandler, IScopePublisher publisher, IMessageRepository msgRepo)
        {
            _logger = logger;
            _lineValidation = lineValidation;
            _env = env;
            _channelSetting = channelSetting;
            _skHandler = skHandler;
            _publisher = publisher;
            _msgRepo = msgRepo;
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

        public IEnumerable<MessageDto> GetMessages(MessageRoute route, MessageParam param)
        {
            var mo_type_format = new BsonArray
            {
                new BsonDocument("$eq", new BsonArray
                {
                    new BsonDocument("$first", "$message_object.events.message.type"),
                    "sticker"
                }),
                new BsonArray
                {
                    new BsonDocument
                    {
                        { "type", "sticker" },
                        { "id", new BsonDocument("$first", "$message_object.events.message.id") },
                        { "stickerId", new BsonDocument("$first", "$message_object.events.message.stickerId") },
                        { "packageId", new BsonDocument("$first", "$message_object.events.message.packageId") },
                        { "stickerResourceType", new BsonDocument("$first", "$message_object.events.message.stickerResourceType") }
                    }
                },
                "$message_object.events.message"
            };

            var match_stage = new BsonDocument
            {
                { "platform_id", param.PlatformId },
                { "group_id", route.GroupId },
                { "message_object.events.message.type", new BsonDocument("$in", BsonArray.Create(param.GetMessageTypes()) ) }
            };

            if (!string.IsNullOrEmpty(param.Text))
            {
                match_stage["message_object.events.message.type"] = "text";
                match_stage["message_object.events.message.text"] = new BsonDocument("$regex", new BsonRegularExpression(param.Text, "i"));
            }
            if (!string.IsNullOrEmpty(param.StartDate))
            {
                match_stage["created_date"] = new BsonDocument("$regex", new BsonRegularExpression(param.StartDate, "i"));
            }

            var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$sort", new BsonDocument(param.GetSortField(), param.GetSortValue())),
                new BsonDocument("$match", match_stage),
                new BsonDocument("$limit", param.Limit),
                new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "lmc_users" },
                    { "localField", "group_user_id" },
                    { "foreignField", "group_user_id" },
                    { "as", "users" }
                }),
                new BsonDocument("$addFields",
                    new BsonDocument
                    {
                        { "group_user_id", new BsonDocument("$first", "$users.group_user_id") },
                        { "display_name", new BsonDocument("$first", "$users.display_name") }
                    }
                ),
                new BsonDocument("$project",
                    new BsonDocument
                    {
                        { "_id", 0 },
                        { "message_id", "$_id" },
                        { "group_id", 1 },
                        {
                            "user",
                            new BsonDocument
                            {
                                { "group_user_id", "$group_user_id" },
                                { "display_name", "$display_name" }
                            }
                        },
                        { "created_date", 1 },
                        { "modified_date", 1 },
                        {
                            "message_object",
                            new BsonDocument("$cond", mo_type_format)
                        }
                    }
                )
            };

            return _msgRepo.FindMessages<MessageDto>(pipeline);
        }
    }
}