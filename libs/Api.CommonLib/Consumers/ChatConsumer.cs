using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.MessageLib.Models;
using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.ReferenceLib.Stores;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Simple.RabbitMQ;

namespace Api.ChatLib.Consumers
{
    public class ChatConsumer : IChatConsumer
    {
        private readonly ILogger<ChatConsumer> _logger;
        private readonly ILineGroupInfo _lineGroupProfile;
        private readonly IChatRepository _chatRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserChatRepository _userChatRepo;
        public ChatConsumer(ILogger<ChatConsumer> logger, IOptions<MongoConfigSetting> mongoConfig,
            ILineGroupInfo lineGroupProfile, IChatRepository chatRepo, IOptions<LineChannelSetting> channelSetting, IServiceProvider serviceProvider, IUserChatRepository userChatRepo)
        {
            _logger = logger;
            _lineGroupProfile = lineGroupProfile;
            _chatRepo = chatRepo;
            _channelSetting = channelSetting;
            _serviceProvider = serviceProvider;
            _userChatRepo = userChatRepo;
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
                Group = new ChatGroupModel
                {
                    GroupId = msgModel.GroupId
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
            ChatModel response = new ChatModel();
            try
            {
                response = await _chatRepo.FindChat(chatModel.Group.GroupId!);
                if (response != null)
                {
                    return;
                }
            }
            catch
            {
                // do nothing
            }


            // chat exist!
            // if(response.StatusCode == StatusCodes.Status409Conflict)
            // {
            //     return;
            // }

            // _logger.LogInformation(_channelSetting.Value.ChannelAccessToken);
            // get group summary from line
            GetGroupSummaryDto groupSummary = await _lineGroupProfile.GetGroupSummary(chatModel.Group.GroupId!, _channelSetting.Value.ChannelAccessToken!);
            // await _chatCols.InsertOneAsync(
            //     BsonDocument.Parse(JsonConvert.SerializeObject(chatModel))
            // );

            chatModel.Group.GroupName = groupSummary.GroupName;
            chatModel.Group.PictureUrl = groupSummary.PictureUrl;

            // add chat in lmc_chat_db
            await _chatRepo.AddChat(chatModel);

            // _logger.LogInformation($"Group name: {groupSummary.GroupName}");

            // publish the chat
            using (var scope = _serviceProvider.CreateScope())
            {
                string routingKey = RoutingKeys.Chat["create"];
                string strChatModel = JsonConvert.SerializeObject(chatModel);
                var publisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();
                publisher.Publish(strChatModel, routingKey, null);
            }
            return;
        }

        public async Task ConsumeUserChatVerify(string message)
        {
            UserChatModel userChatModel = JsonConvert
                .DeserializeObject<UserChatModel>(message)!;

            ChatModel chatModel = new ChatModel();
            try
            {
                chatModel = await _chatRepo.FindChat(userChatModel.GroupId!);
                if (chatModel != null)
                    return;
            }
            catch
            {
                GetGroupSummaryDto groupSummary = await _lineGroupProfile.GetGroupSummary(userChatModel.GroupId!, _channelSetting.Value.ChannelAccessToken!);
                chatModel=new ChatModel{
                    Group=new ChatGroupModel{
                        GroupId=userChatModel.GroupId,
                        GroupName=groupSummary.GroupName,
                        PictureUrl=groupSummary.PictureUrl
                    },
                    PlatformId=userChatModel.PlatformId
                };
                await _chatRepo.AddChat(chatModel);
                await _userChatRepo.AddUserChat(userChatModel);
            }
        }
    }
}