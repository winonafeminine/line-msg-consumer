namespace Api.ReferenceLib.Stores
{
    public class MessageEventTypes
    {
        public static string Text = "text";
        public static string Sticker = "sticker";
        public static string Image = "image";
        public static string Video = "video";
        public static string Audio = "audio";
        public static string File = "file";
        public static string Location = "location";
        public static List<string> AllTypes = new List<string>{
            Text,
            Sticker,
            Image,
            Video,
            Audio,
            File,
            Location
        };
    }
}