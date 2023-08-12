using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.MessageLib.Protos;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Models;
using Api.ReferenceLib.Settings;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.MessageLib.Grpcs
{
    public class MessageGrpcClientService : IMessageGrpcClientService
    {
        private readonly IOptions<GrpcConfigSetting> _grpcConfig;
        private readonly ILogger<MessageGrpcClientService> _logger;
        public MessageGrpcClientService(IOptions<GrpcConfigSetting> grpcConfig, ILogger<MessageGrpcClientService> logger)
        {
            _grpcConfig = grpcConfig;
            _logger = logger;
        }
        public async Task<Response> AddMessage(MessageModel model)
        {
            string address = _grpcConfig.Value.Message!.Address!;
            using var channel = GrpcChannel.ForAddress(address);
            _logger.LogInformation(address);
            var client = new MessageGrpc.MessageGrpcClient(channel);
            MessageGrpcReply reply = new MessageGrpcReply();

            string strMessage = JsonConvert.SerializeObject(model);

            try
            {
                reply = await client.AddMessageAsync(
                                new AddMessageGrpcRequest { Data = strMessage });
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