using Api.LmcLib.DTOs;
using Api.LmcLib.Parameters;
using Api.LmcLib.Routes;

namespace Api.LmcLib.Interfaces
{
    public interface IMessageService
    {
        public Task RetriveLineMessage(object content, string signature, string id);
        public IEnumerable<MessageDto> GetMessages(MessageRoute route, MessageParam param);
    }
}