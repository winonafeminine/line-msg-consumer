namespace Api.CommonLib.Interfaces
{
    public interface IChatConsumer
    {
        public Task ConsumeMessageCreate(string message);
        public Task ConsumeUserChatVerify(string message);
    }
}