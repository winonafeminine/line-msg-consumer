namespace Api.ReferenceLib.Interfaces
{
    public interface IMessageConsumer
    {
        public Task ConsumeMessageCreate(string message);
    }
}