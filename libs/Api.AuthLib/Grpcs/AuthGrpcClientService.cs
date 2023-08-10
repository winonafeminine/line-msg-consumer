using Api.AuthLib.Interfaces;
using Api.AuthLib.Models;
using Api.AuthLib.Protos;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Settings;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.AuthLib.Grpcs
{
    public class AuthGrpcClientService : IAuthGrpcClientService
    {
        private readonly ILogger<AuthGrpcClientService> _logger;
        private readonly IOptions<GrpcConfigSetting> _grpcConfig;
        public AuthGrpcClientService(ILogger<AuthGrpcClientService> logger, IOptions<GrpcConfigSetting> grpcConfig)
        {
            _logger = logger;
            _grpcConfig = grpcConfig;
        }
        public async Task<JwtPayloadData> ValidateJwtToken(string token)
        {
            using (var channel = GrpcChannel.ForAddress(_grpcConfig.Value.Auth!.Address!))
            {
                var client = new AuthGrpc.AuthGrpcClient(channel);
                AuthGrpcReply reply = new AuthGrpcReply();

                try
                {
                    reply = await client.ValidateJwtTokenAsync(
                                    new GrpcValidateJwtTokenRequest { Token = token });
                }
                catch (RpcException ex)
                {
                    int statusCode = StatusCodes.Status401Unauthorized;

                    _logger.LogError(ex.Status.Detail);
                    throw new ErrorResponseException(
                        statusCode,
                        ex.Status.Detail,
                        new List<Error>()
                    );
                }
                return JsonConvert.DeserializeObject<JwtPayloadData>(reply.Data)!;
            }
        }
    }
}