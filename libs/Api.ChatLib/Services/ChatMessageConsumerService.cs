using Api.CommonLib.DTOs;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Api.CommonLib.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.ChatLib.Services
{
    public class ChatMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<ChatMessageConsumerService> _logger;
        private readonly ILineGroupInfo _lineGroupProfile;
        private readonly IChatRepository _chatRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IServiceProvider _serviceProvider;
        public ChatMessageConsumerService(ILogger<ChatMessageConsumerService> logger, IOptions<MongoConfigSetting> mongoConfig,
            ILineGroupInfo lineGroupProfile, IChatRepository chatRepo, IOptions<LineChannelSetting> channelSetting, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _lineGroupProfile = lineGroupProfile;
            _chatRepo = chatRepo;
            _channelSetting = channelSetting;
            _serviceProvider = serviceProvider;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;
            
            // _logger.LogInformation(message);

            ChatModel chatModel = new ChatModel
            {
                Group = new ChatGroupModel{
                    GroupId=msgModel.GroupId
                },
                LatestMessage = new ChatLatestMessageModel
                {
                    MessageId = msgModel.MessageId,
                    GroupUserId = msgModel.GroupUserId,
                    LatestDate = msgModel.CreatedDate,
                    // get the text from message object
                }
            };

            // check if group existed
            Response response = _chatRepo.FindChat(chatModel.Group.GroupId!);
            // chat exist!
            if(response.StatusCode == StatusCodes.Status409Conflict)
            {
                return;
            }

            // _logger.LogInformation(_channelSetting.Value.ChannelAccessToken);
            // get group summary from line
            GetGroupSummaryDto groupSummary = await _lineGroupProfile.GetGroupSummary(chatModel.Group.GroupId!, _channelSetting.Value.ChannelAccessToken!);
            // await _chatCols.InsertOneAsync(
            //     BsonDocument.Parse(JsonConvert.SerializeObject(chatModel))
            // );
            
            chatModel.Group.GroupName=groupSummary.GroupName;
            chatModel.Group.PictureUrl=groupSummary.PictureUrl;

            // add chat in lmc_chat_db
            response = _chatRepo.AddChat(chatModel);

            // _logger.LogInformation($"Group name: {groupSummary.GroupName}");

            // publish the chat
            using(var scope = _serviceProvider.CreateScope())
            {
                string routingKey = RoutingKeys.Chat["create"];
                string strChatModel = JsonConvert.SerializeObject(chatModel);
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
                publisher.Publish(strChatModel, routingKey, null);
            }
            return;
        }
    }
}