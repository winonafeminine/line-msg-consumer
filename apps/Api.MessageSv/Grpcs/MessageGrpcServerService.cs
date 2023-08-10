using Api.MessageLib.Interfaces;
using Api.MessageLib.Models;
using Api.MessageLib.Protos;
using Api.ReferenceLib.Models;
using Grpc.Core;
using Newtonsoft.Json;

namespace Api.MessageSv.Grpcs
{
    public class MessageGrpcServerService : MessageGrpc.MessageGrpcBase
    {
        private readonly ILogger<MessageGrpcServerService> _logger;
        private readonly IMessageRepository _msgRepo;
        public MessageGrpcServerService(ILogger<MessageGrpcServerService> logger, IMessageRepository msgRepo)
        {
            _logger = logger;
            _msgRepo = msgRepo;
        }

        public override async Task<MessageGrpcReply> AddMessage(AddMessageGrpcRequest request, ServerCallContext context)
        {
            MessageModel messageModel = JsonConvert.DeserializeObject<MessageModel>(request.Data)!;

            Response response = await _msgRepo.AddMessage(messageModel);

            _logger.LogInformation(response.Message);
            return new MessageGrpcReply
            {
                StatusCode = ((int)StatusCode.OK),
                Message = response.Message,
                Data = JsonConvert.SerializeObject(messageModel)
            };
        }
    }
}