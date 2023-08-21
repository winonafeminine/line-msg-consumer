namespace Api.CommonLib.Interfaces
{
    public interface IMessageConsumer
    {
        public Task ConsumeMessageCreate(string message);
    }
}