namespace Api.UserLib.Interfaces
{
    public interface IUserMessage
    {
        public Task HandleMessageCreate(string message);
    }
}