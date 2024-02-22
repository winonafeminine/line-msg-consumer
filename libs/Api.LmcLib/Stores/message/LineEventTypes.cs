namespace Api.LmcLib.Models
{
    public class LineEventTypes
    {
        public static string Message = "message";
        public static string MemberJoined = "memberJoined";
        public static List<string> AllTypes = new List<string>{
            Message
        };
    }
}