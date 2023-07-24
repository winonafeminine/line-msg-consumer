using Api.MessageLib.Settings;

namespace Api.MessageLib.Interfaces
{
    public interface IMessageRpcClient
    {
        public LineChannelSetting GetChannel();
    }
}