using Api.LmcLib.Interfaces;
using Api.LmcLib.Models;
using Api.LmcLib.DTOs;
using Api.LmcLib.Setttings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.LmcLib.Consumers
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
            
            ChatModel chat = new ChatModel{
                Group = new ChatGroupModel{
                    GroupId=userChatModel.GroupId,
                },
                PlatformId = userChatModel.PlatformId
                
                
            };

            GetGroupSummaryDto groupInfo = new GetGroupSummaryDto();
            try{

                groupInfo = await _lineGroupProfile.GetGroupSummary(userChatModel.GroupId!, _channelSetting.Value.ChannelAccessToken!);
            }catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return;
            }

            chat.Group!.GroupName=groupInfo.GroupName;
            chat.Group!.PictureUrl=groupInfo.PictureUrl;

            await _chatRepo.AddChat(chat);
            await _userChatRepo.AddUserChat(userChatModel);
        }
    }
}