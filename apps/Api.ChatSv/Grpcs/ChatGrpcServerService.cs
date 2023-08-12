using Api.ChatLib.Protos;
using Api.CommonLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Interfaces;
using Grpc.Core;
using Newtonsoft.Json;

namespace Api.ChatSv.Grpcs
{
    public class ChatGrpcServerService : ChatGrpc.ChatGrpcBase
    {
        private readonly ILogger<ChatGrpcServerService> _logger;
        private readonly IChatRepository _chatRepo;
        public ChatGrpcServerService(ILogger<ChatGrpcServerService> logger, IChatRepository chatRepo)
        {
            _logger = logger;
            _chatRepo = chatRepo;
        }
        public override async Task<ChatGrpcReply> GetChat(ChatGrpcRequest request, ServerCallContext context)
        {
            ChatModel chat = new ChatModel();

            try{
                chat = await _chatRepo.FindChat(request.GroupId);
            }catch(ErrorResponseException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Description!), ex.Description!);
            }

            return new ChatGrpcReply{
                Message="Successfully get the chat",
                StatusCode=((int)StatusCode.OK),
                Data=JsonConvert.SerializeObject(chat)
            };
        }
    }
}