using Api.CommonLib.Models;
using Api.CommonLib.Setttings;

namespace Api.CommonLib.Interfaces
{
    public interface IMessageRpcClient
    {
        public LineChannelSetting GetChannel();
        public Response AddUser(UserModel user);
    }
}