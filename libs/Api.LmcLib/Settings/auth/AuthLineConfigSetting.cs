namespace Api.LmcLib.Settings
{
    public class AuthLineConfigSetting
    {
        public virtual string? ClientId { get; set; }
        public virtual string? RedirectUri { get; set; }
        public virtual string? SecretId { get; set; }
    }
}