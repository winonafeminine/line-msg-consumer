using Api.ReferenceLib.Exceptions;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Api.UserLib.Protos;
using Grpc.Core;
using Newtonsoft.Json;

namespace Api.ChatSv.Grpcs
{
    public class UserChatGrpcServerService : UserChatGrpc.UserChatGrpcBase
    {
        private readonly ILogger<UserChatGrpcServerService> _logger;
        private readonly IUserChatRepository _userChatRepo;
        public UserChatGrpcServerService(ILogger<UserChatGrpcServerService> logger, IUserChatRepository userChatRepo)
        {
            _logger = logger;
            _userChatRepo = userChatRepo;
        }

        public override async Task<UserChatGrpcReply> AddUserChat(AddUserChatGrpcRequest request, ServerCallContext context)
        {
            try
            {
                UserChatModel userChat = JsonConvert
                    .DeserializeObject<UserChatModel>(request.Data)!;

                await _userChatRepo.AddUserChat(userChat);
            }
            catch (Exception ex)
            {
                if (ex is ErrorResponseException err)
                {
                    throw new RpcException(new Status(StatusCode.FailedPrecondition, err.Description!), err.Description!);
                }
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message), ex.Message);
            }
            return new UserChatGrpcReply
            {
                Data = request.Data,
                StatusCode = ((int)StatusCode.OK)
            };
        }
    }
}