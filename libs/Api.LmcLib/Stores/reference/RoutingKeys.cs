namespace Api.LmcLib.Stores
{
    public class RoutingKeys
    {
        public static IDictionary<string, string> Message = new Dictionary<string, string>{
            { "create", "lmc.message.create" },
            { "verify", "lmc.message.verify" }
        };
        public static IDictionary<string, string> User = new Dictionary<string, string>{
            { "create", "lmc.user.create" }
        };
        public static IDictionary<string, string> Chat = new Dictionary<string, string>{
            { "create", "lmc.chat.create" }
        };
        public static IDictionary<string, string> UserChat = new Dictionary<string, string>{
            { "create", "lmc.user_chat.create" },
            { "verify", "lmc.user_chat.verify" },
        };
        public static IDictionary<string, string> Auth = new Dictionary<string, string>{
            { "update", "lmc.auth.update" }
        };
        public static IDictionary<string, string> Platform = new Dictionary<string, string>{
            { "verify", "lmc.auth.verify" }
        };
    }
}