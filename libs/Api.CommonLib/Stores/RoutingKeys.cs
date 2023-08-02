namespace Api.CommonLib.Stores
{
    public class RoutingKeys
    {
        public static IDictionary<string, string> Message = new Dictionary<string, string>{
            { "create", "lmc.message.create" }
        };
        public static IDictionary<string, string> User = new Dictionary<string, string>{
            { "create", "lmc.user.create" }
        };
        public static IDictionary<string, string> Chat = new Dictionary<string, string>{
            { "create", "lmc.chat.create" }
        };
    }
}