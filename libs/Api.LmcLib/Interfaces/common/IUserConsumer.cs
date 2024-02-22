namespace Api.LmcLib.Interfaces
{
    public interface IUserConsumer
    {
        public Task ConsumeMessageCreate(string message);
        public Task ConsumePlatformVerify(string message);
        public Task ConsumeAuthUpdate(string message);
        public Task ConsumeMessageVerify(string message);
    }
}