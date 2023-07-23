using Api.ChatLib.DTOs;
using Api.ChatLib.Models;
using Api.CommonLib.Interfaces;
using Api.MessageLib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.ChatLib.Services
{
    public class ChatMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<ChatMessageConsumerService> _logger;
        public ChatMessageConsumerService(ILogger<ChatMessageConsumerService> logger)
        {
            _logger = logger;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;

            ChatModel chatModel = new ChatModel
            {
                Group = new ChatGroupDto{
                    GroupId=msgModel.GroupId
                },
                LatestMessage = new ChatLatestMessageDto
                {
                    MessageId = msgModel.MessageId,
                    LineUserId = msgModel.LineUserId,
                    LatestDate = msgModel.CreatedDate,
                    // get the text from message object
                }
            };

            _logger.LogInformation($"Chat model: {JsonConvert.SerializeObject(chatModel)}");
            return;
        }
    }
}