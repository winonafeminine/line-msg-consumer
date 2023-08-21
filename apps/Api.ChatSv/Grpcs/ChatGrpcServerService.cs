using Api.ChatLib.Protos;
using Api.CommonLib.Models;
using Api.ReferenceLib.DTOs;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Api.ReferenceLib.Setttings;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.ChatSv.Grpcs
{
    public class ChatGrpcServerService : ChatGrpc.ChatGrpcBase
    {
        private readonly ILogger<ChatGrpcServerService> _logger;
        private readonly IChatRepository _chatRepo;
        private readonly IOptions<LineChannelSetting> _channelSetting;
        private readonly ILineGroupInfo _lineGroupProfile;
        public ChatGrpcServerService(ILogger<ChatGrpcServerService> logger, IChatRepository chatRepo, IOptions<LineChannelSetting> channelSetting, ILineGroupInfo lineGroupProfile)
        {
            _logger = logger;
            _chatRepo = chatRepo;
            _channelSetting = channelSetting;
            _lineGroupProfile = lineGroupProfile;
        }
        public override async Task<ChatGrpcReply> GetChat(ChatGrpcRequest request, ServerCallContext context)
        {
            ChatModel chat = new ChatModel();

            try
            {
                chat = await _chatRepo.FindChat(request.GroupId);
            }
            catch (ErrorResponseException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Description!), ex.Description!);
            }

            return new ChatGrpcReply
            {
                Message = "Successfully get the chat",
                StatusCode = ((int)StatusCode.OK),
                Data = JsonConvert.SerializeObject(chat)
            };
        }

        public override async Task<ChatGrpcReply> AddChat(AddChatGrpcRequest request, ServerCallContext context)
        {
            try
            {
                ChatModel chat = JsonConvert
                    .DeserializeObject<ChatModel>(request.Data)!;

                GetGroupSummaryDto groupInfo = new GetGroupSummaryDto();
                groupInfo = await _lineGroupProfile.GetGroupSummary(chat.Group!.GroupId!, _channelSetting.Value.ChannelAccessToken!);

                chat.Group!.GroupName = groupInfo.GroupName;
                chat.Group!.PictureUrl = groupInfo.PictureUrl;
                await _chatRepo.AddChat(chat);
            }
            catch (Exception ex)
            {
                if (ex is ErrorResponseException err)
                {
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, err.Description!), err.Description!);
                }
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message!), ex.Message!);
            }
            return new ChatGrpcReply
            {
                Data = request.Data,
                StatusCode = ((int)StatusCode.OK)
            };
        }
    }
}