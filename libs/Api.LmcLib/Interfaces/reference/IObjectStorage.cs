using Api.LmcLib.DTOs;

namespace Api.LmcLib.Interfaces
{
    public interface IObjectStorage
    {
        public Task<string> UploadFile(StaticfileDto staticfileDto);
    }
}