namespace Api.CommonLib.Interfaces
{
    public interface IChatConsumer
    {
        public Task ConsumeUserChatVerify(string message);
    }
}