using Api.MessageLib.Models;
using Api.UserLib.DTOs;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Api.UserLib.Services
{
    public class UserMessageService : IUserMessage
    {
        private readonly ILogger<UserMessageService> _logger;
        public UserMessageService(ILogger<UserMessageService> logger)
        {
            _logger = logger;
        }
        public async Task HandleMessageCreate(string message)
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