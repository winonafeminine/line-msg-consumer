namespace Api.LmcLib.Interfaces
{
    public interface IChatConsumer
    {
        public Task ConsumeUserChatVerify(string message);
    }
}