namespace Api.LmcLib.Interfaces
{
    public interface IMessageConsumer
    {
        public Task ConsumeMessageCreate(string message);
    }
}