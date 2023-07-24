namespace Api.MessageLib.Interfaces
{
    public interface IMessageRpcClient : IDisposable
    {
        public Task<string> CallAsync(string message, CancellationToken cancellationToken = default);
    }
}