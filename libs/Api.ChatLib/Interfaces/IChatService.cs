using Api.ChatLib.DTOs;
using Api.ChatLib.Parameters;

namespace Api.ChatLib.Interfaces
{
    public interface IChatService
    {
        public IEnumerable<ChatDto> GetChats(ChatParam param);
    }
}