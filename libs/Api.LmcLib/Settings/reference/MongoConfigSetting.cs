namespace Api.LmcLib.Setttings
{
    public class MongoConfigSetting
    {
        public virtual string? HostName { get; set; }
        public virtual string? DatabaseName { get; set; }
        public static IDictionary<string, string> Collections = new Dictionary<string, string>{
            { "Message", "lmc_messages" },
            { "User", "lmc_users" },
            { "Chat", "lmc_chats" },
            { "UserChat", "lmc_user_chats" },
            { "Platform", "lmc_platforms" },
            { "Auth", "lmc_auths" },
        };
    }
}