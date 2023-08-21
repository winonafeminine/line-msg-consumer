using Api.ReferenceLib.DTOs;

namespace Api.ReferenceLib.Interfaces
{
    public interface IObjectStorage
    {
        public Task<string> UploadFile(StaticfileDto staticfileDto);
    }
}