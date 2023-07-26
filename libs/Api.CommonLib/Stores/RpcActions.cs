namespace Api.CommonLib.Stores
{
    public class RpcActions
    {
        public static IDictionary<string, string> Message = new Dictionary<string, string>{
            { "GetChannel", "GetChannelAction" },
            { "CreateUser", "CreateUserAction" },
            { "CreateChat", "CreateChatAction" },
        };
    }
}