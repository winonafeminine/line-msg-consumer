using Api.ChatLib.Interfaces;
using Api.ChatLib.Protos;
using Api.CommonLib.Models;
using Api.ReferenceLib.Exceptions;
using Api.ReferenceLib.Settings;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.ChatLib.Grpcs
{
    public class ChatGrpcClientService : IChatGrpcClientService
    {
        private readonly IOptions<GrpcConfigSetting> _grpcConfig;
        private readonly ILogger<ChatGrpcClientService> _logger;
        public ChatGrpcClientService(IOptions<GrpcConfigSetting> grpcConfig, ILogger<ChatGrpcClientService> logger)
        {
            _grpcConfig = grpcConfig;
            _logger = logger;
        }
        public async Task<ChatModel> GetChat(string groupId)
        {
            string address = _grpcConfig.Value.Chat!.Address!;
            using var channel = GrpcChannel.ForAddress(address);
            _logger.LogInformation(address);
            var client = new ChatGrpc.ChatGrpcClient(channel);
            ChatGrpcReply reply = new ChatGrpcReply();

            try
            {
                reply = await client.GetChatAsync(
                                new ChatGrpcRequest { GroupId = groupId });
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
            return JsonConvert
                .DeserializeObject<ChatModel>(reply.Data)!;
        }
    }
}