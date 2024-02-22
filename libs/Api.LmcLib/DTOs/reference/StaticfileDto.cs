namespace Api.LmcLib.DTOs
{
    public class StaticfileDto
    {
        public virtual string? Key { get; set; }
        public virtual string? ContentType { get; set; }
        public virtual string? FilePath { get; set; }
    }
}