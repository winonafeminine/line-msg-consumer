using Api.MessageLib.DTOs;
using Api.MessageLib.Parameters;
using Api.MessageLib.Routes;

namespace Api.MessageLib.Interfaces
{
    public interface IMessageService
    {
        public Task RetriveLineMessage(object content, string signature, string id);
        public IEnumerable<MessageDto> GetMessages(MessageRoute route, MessageParam param);
    }
}