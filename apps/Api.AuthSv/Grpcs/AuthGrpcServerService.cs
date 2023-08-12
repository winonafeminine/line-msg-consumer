using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.AuthLib.Protos;
using Api.PlatformLib.Interfaces;
using Api.PlatformLib.Models;
using Api.ReferenceLib.Exceptions;
using Grpc.Core;
using Newtonsoft.Json;

namespace Api.AuthSv.Grpcs
{
    public class AuthGrpcServerService : AuthGrpc.AuthGrpcBase
    {
        private readonly ILogger<AuthGrpcServerService> _logger;
        private readonly IPlatformRepository _platformRepo;
        private readonly IJwtToken _jwtToken;
        public AuthGrpcServerService(ILogger<AuthGrpcServerService> logger, IPlatformRepository platformRepo, IJwtToken jwtToken)
        {
            _logger = logger;
            _platformRepo = platformRepo;
            _jwtToken = jwtToken;
        }

        public override async Task<AuthGrpcReply> ValidateJwtToken(GrpcValidateJwtTokenRequest request, ServerCallContext context)
        {
            JwtPayloadData  payload = new JwtPayloadData();
            PlatformModel platformModel = new PlatformModel();

            // validating the token
            try{
                payload = _jwtToken.GetJwtPayloadData(request.Token);

                platformModel = await _platformRepo.Find(payload.PlatformId!);
                // if issuer=platform
                _jwtToken.ValidateJwtToken(request.Token, platformModel.SecretKey!);

            }catch(Exception ex){

                if(ex is ErrorResponseException exRes)
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, exRes.Description!), exRes.Description!);
                }
                
                throw new RpcException(new Status(StatusCode.PermissionDenied, ex.Message!), ex.Message!);
            }

            return new AuthGrpcReply{
                StatusCode=(int?)(StatusCode.OK),
                Message="Successfully validate the token!",
                Data=JsonConvert.SerializeObject(payload)
            };
        }
    }
}