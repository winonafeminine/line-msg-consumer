using Api.ReferenceLib.Setttings;

namespace Api.MessageLib.Interfaces
{
    public interface IMessageService
    {
        public Task RetriveLineMessage(object content, string signature, string id);
        public LineChannelSetting GetChannel();
    }
}