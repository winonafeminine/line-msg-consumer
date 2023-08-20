using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Settings;
using Api.UserLib.Interfaces;
using Api.UserLib.Models;
using Api.UserLib.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.UserLib.Grpcs
{
    public class UserGrpcClientService : IUserGrpcClientService
    {
        private readonly ILogger<UserGrpcClientService> _logger;
        private readonly IOptions<GrpcConfigSetting> _grpcConfig;
        public UserGrpcClientService(ILogger<UserGrpcClientService> logger, IOptions<GrpcConfigSetting> grpcConfig)
        {
            _logger = logger;
            _grpcConfig = grpcConfig;
        }
        public async Task<Response> AddUser(UserModel model)
        {
            string address = _grpcConfig.Value.Message!.Address!;
            using var channel = GrpcChannel.ForAddress(address);
            _logger.LogInformation(address);
            var client = new UserGrpc.UserGrpcClient(channel);
            UserGrpcReply reply = new UserGrpcReply();

            string strMessage = JsonConvert.SerializeObject(model);

            try
            {
                reply = await client.AddUserAsync(
                                new AddUserGrpcRequest { Data = strMessage });
            }
            catch (RpcException ex)
            {
                int statusCode = StatusCodes.Status400BadRequest;

                _logger.LogError(ex.Status.Detail);
                throw new ErrorResponseException(
                    statusCode,
                    ex.Status.Detail,
                    new List<Error>()
                );
            }
            return new Response
            {
                StatusCode = StatusCodes.Status200OK,
                Message = reply.Message
            };
        }
    }
}