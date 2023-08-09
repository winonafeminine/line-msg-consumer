namespace Api.CommonLib.Interfaces
{
    public interface IUserConsumer
    {
        public Task ConsumeMessageCreate(string message);
        public Task ConsumePlatformVerify(string message);
    }
}