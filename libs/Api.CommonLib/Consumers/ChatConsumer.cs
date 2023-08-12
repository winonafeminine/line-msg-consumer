using Api.CommonLib.Interfaces;
using Api.CommonLib.Models;
using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

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
        public async Task ConsumeUserChatVerify(string message)
        {
            UserChatModel userChatModel = JsonConvert
                .DeserializeObject<UserChatModel>(message)!;

            await _userChatRepo.AddUserChat(userChatModel);
        }
    }
}