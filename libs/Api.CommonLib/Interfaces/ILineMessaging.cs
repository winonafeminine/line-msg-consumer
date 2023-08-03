using Api.ReferenceLib.Models;
using Api.ReferenceLib.Setttings;
using Api.UserLib.Models;

namespace Api.CommonLib.Interfaces
{
    public interface ILineMessaging
    {
        public Task RetriveLineMessage(object content, string signature, string id);
        public LineChannelSetting GetChannel();
        public Response AddUser(UserModel user);
    }
}