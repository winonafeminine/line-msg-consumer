namespace Api.ReferenceLib.Settings
{
    public class GrpcConfigSetting
    {
        public AuthGrpcConfigSetting? Auth { get; set; }
        public MessageGrpcConfigSetting? Message { get; set; }
    }

    public class CommonGrpcConfigSetting
    {
        public string? Address { get; set; }
    }

    public class AuthGrpcConfigSetting : CommonGrpcConfigSetting{}
    public class MessageGrpcConfigSetting : CommonGrpcConfigSetting{}
}