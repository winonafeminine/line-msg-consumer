using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Api.UserLib.Protos;
using Grpc.Core;
using Newtonsoft.Json;

namespace Api.MessageSv.Grpcs
{
    public class UserGrpcServerService : UserGrpc.UserGrpcBase
    {
        private readonly ILogger<UserGrpcServerService> _logger;
        private readonly IUserRepository _userRepo;
        public UserGrpcServerService(ILogger<UserGrpcServerService> logger, IUserRepository userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        public override async Task<UserGrpcReply> AddUser(AddUserGrpcRequest request, ServerCallContext context)
        {
            var userModel = new UserModel();

            try{
                userModel = JsonConvert
                    .DeserializeObject<UserModel>(request.Data);
                
                await _userRepo.AddUser(userModel!);
            }catch(Exception ex)
            {
                throw new RpcException(new Status(StatusCode.FailedPrecondition, ex.Message), ex.Message);
            }

            var response = new UserGrpcReply{
                Data=request.Data,
                StatusCode=((int)StatusCode.OK)
            };
            return response;
        }
    }
}