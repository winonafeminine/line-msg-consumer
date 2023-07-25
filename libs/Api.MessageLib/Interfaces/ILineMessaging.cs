
using Api.CommonLib.Models;
using Api.CommonLib.Setttings;

namespace Api.MessageLib.Interfaces
{
    public interface ILineMessaging
    {
        public Task RetriveLineMessage(object content, string signature, string id);
        public LineChannelSetting GetChannel();
        public Response AddUser(UserModel user);
    }
}