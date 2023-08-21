namespace Api.ReferenceLib.Stores
{
    public class LineApiReference
    {
        public static string ApiUrl = "https://api.line.me";
        public static string DataApiUrl = "https://api-data.line.me";
        public static string AuthUrl = "https://access.line.me/oauth2";
        public static string ApiVersion = "v2";
        public static string DataApiVersion = "v2";
        public static string LoginVersion = "v2.1";
        public static string Oauth = "oauth2";
        public static string Bot = "bot";
        public static string Group = "group";
        public static string Member = "member";
        public static string Summary = "summary";
        public static string Authorize = "authorize";
        public static string Token = "token";
        public static string Verify = "verify";
        public static string Profile = "profile";
        public static string Message = "message";
        public static string Content = "content";
        public static string GetGroupMemberProfile(string groupId, string userId)
        {
            return Path.Combine(ApiUrl, ApiVersion, Bot, Group, groupId, Member, userId);
        }
        public static string GetGroupSummary(string groupId)
        {
            return Path.Combine(ApiUrl, ApiVersion, Bot, Group, groupId, Summary);
        }
        
        // https://access.line.me/oauth2/v2.1/authorize?response_type=code&client_id=1234567890&redirect_uri=https%3A%2F%2Fexample.com%2Fauth%3Fkey%3Dvalue&state=12345abcde&scope=profile%20openid&nonce=09876xyz
        public static string GetLineAuthorizationUrl(string clientId, string redirectUri, string state)
        {
            string responseType = "code";
            string scope = "profile%20openid%20email";
            string queryParam = $"{Authorize}?response_type={responseType}&client_id={clientId}&redirect_uri={redirectUri}&state={state}&scope={scope}";
            return Path.Combine(AuthUrl, LoginVersion, queryParam);
        }
        public static string GetLineLoginIssueTokenUrl()
        {
            return Path.Combine(ApiUrl, Oauth, LoginVersion, Token);
        }
        public static string GetLineLoginTokenVerifyUrl(string accessToken)
        {
            string queryParam = $"{Verify}?access_token={accessToken}";
            return Path.Combine(ApiUrl, Oauth, LoginVersion, queryParam);
        }
        public static string GetLineLoginUserProfileUrl()
        {
            return Path.Combine(ApiUrl, ApiVersion, Profile);
        }
        public static string GetContent(string messageId)
        {
            // $"https://api-data.line.me/v2/bot/message/{messageId}/content"
            return Path.Combine(DataApiUrl, ApiVersion, Bot, Message, messageId, Content);
        }
        public static string GetStickerUrl(string stickerId)
        {
            // https://stickershop.line-scdn.net/stickershop/v1/sticker/52002736/android/sticker.png
            return $"https://stickershop.line-scdn.net/stickershop/v1/sticker/{stickerId}/android/sticker.png";
        }
    }
}