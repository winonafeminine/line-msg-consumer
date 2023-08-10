using Api.MessageLib.Models;
using Api.ReferenceLib.Models;

namespace Api.MessageLib.Interfaces
{
    public interface IMessageGrpcClientService
    {
        public Task<Response> AddMessage(MessageModel model);
    }
}