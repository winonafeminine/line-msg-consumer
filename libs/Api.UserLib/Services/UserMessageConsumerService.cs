using Api.CommonLib.Interfaces;
using Api.MessageLib.Models;
using Api.UserLib.DTOs;
using Api.UserLib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.UserLib.Services
{
    public class UserMessageConsumerService : IMessageConsumer
    {
        private readonly ILogger<UserMessageConsumerService> _logger;
        public UserMessageConsumerService(ILogger<UserMessageConsumerService> logger)
        {
            _logger = logger;
        }
        public async Task ConsumeMessageCreate(string message)
        {
            await Task.Yield();
            // mapping to the MessageModel
            MessageModel msgModel = JsonConvert
                .DeserializeObject<MessageModel>(message)!;
            
            UserModel userModel = new UserModel{
                ClientId=msgModel.ClientId,
                GroupId=msgModel.GroupId,
                LineUserId=msgModel.LineUserId,
                LatestMessage=new UserLatestMessageDto{
                    IsRead=false,
                    MessageId=msgModel.MessageId
                }
            };

            _logger.LogInformation($"User model: {JsonConvert.SerializeObject(userModel)}");
            return;
        }
    }
}