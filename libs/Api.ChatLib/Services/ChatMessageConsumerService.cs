using Api.CommonLib.DTOs;
using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.ChatLib.Services
{
    public class ChatMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<ChatMessageConsumerService> _logger;
        private readonly ILineGroupInfo _lineGroupProfile;
        private readonly IMessageRpcClient _msgRpcClient;
        private readonly IChatRepository _chatRepo;
        public ChatMessageConsumerService(ILogger<ChatMessageConsumerService> logger, IOptions<MongoConfigSetting> mongoConfig, 
            ILineGroupInfo lineGroupProfile, IMessageRpcClient msgRpcClient, IChatRepository chatRepo)
        {
            _logger = logger;
            _lineGroupProfile = lineGroupProfile;
            _msgRpcClient = msgRpcClient;
            _chatRepo = chatRepo;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            // await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;

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

            // get the channel detail
            var channelResponse = new LineChannelSetting();
            try{
                channelResponse = _msgRpcClient.GetChannel();
            }catch{
                _logger.LogError("Failed getting channel");
            }

            // get group summary from line
            GetGroupSummaryDto groupSummary = await _lineGroupProfile.GetGroupSummary(chatModel.Group.GroupId!, channelResponse.ChannelAccessToken!);
            // await _chatCols.InsertOneAsync(
            //     BsonDocument.Parse(JsonConvert.SerializeObject(chatModel))
            // );
            
            
            chatModel.Group.GroupName=groupSummary.GroupName;
            chatModel.Group.PictureUrl=groupSummary.PictureUrl;

            // add chat in lmc_chat_db
            Response response = _chatRepo.AddChat(chatModel);

            // chat exist!
            if(response.StatusCode == StatusCodes.Status409Conflict)
            {
                return;
            }

            // add chat in lmc_message_db
            Response chatReponse = new Response();
            
            try{
                chatReponse = _msgRpcClient.AddChat(chatModel);
            }catch{
                _logger.LogInformation("Failed adding chat!");
            }
            
            _logger.LogInformation($"Chat saved!");
            return;
        }
    }
}