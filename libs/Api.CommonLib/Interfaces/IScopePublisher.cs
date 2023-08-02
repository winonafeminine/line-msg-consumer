namespace Api.CommonLib.Interfaces
{
    public interface IScopePublisher
    {
        public void Publish(string message, string routingKey, IDictionary<string, object>? headers);
    }
}