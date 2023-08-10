namespace Api.CommonLib.Interfaces
{
    public interface IAuthConsumer
    {
        public Task ConsumeAuthUpdate(string message);
        public Task ConsumePlatformVerify(string message);
    }
}