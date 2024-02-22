using Api.LmcLib.DTOs;
using Api.LmcLib.Parameters;

namespace Api.LmcLib.Interfaces
{
    public interface IChatService
    {
        public IEnumerable<ChatDto> GetChats(ChatParam param);
    }
}