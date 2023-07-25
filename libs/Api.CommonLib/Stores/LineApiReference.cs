namespace Api.CommonLib.Stores
{
    public class LineApiReference
    {
        public static string ApiUrl = "https://api.line.me";
        public static string DataApiUrl = "https://api-data.line.me";
        public static string ApiVersion = "v2";
        public static string DataApiVersion = "v2";
        public static string Bot = "bot";
        public static string Group = "group";
        public static string Member = "member";
        public static string GetGroupMemberProfile(string groupId, string userId)
        {
            return Path.Combine(ApiUrl, ApiVersion, Bot, Group, groupId, Member, userId);
        }
    }
}