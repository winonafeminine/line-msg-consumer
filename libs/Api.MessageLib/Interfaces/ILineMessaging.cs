namespace Api.MessageLib.Interfaces
{
    public interface ILineMessaging
    {
        public Task RetriveLineMessage(object content, string signature, string id);
    }
}