using TimeZoneConverter;

namespace Api.ReferenceLib.Services
{
    public class TimezoneService
    {
        public static DateTimeOffset Convert()
        {
            TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo("Asia/Bangkok");
            DateTimeOffset currentTime = DateTimeOffset.UtcNow.ToOffset(tzi.GetUtcOffset(DateTimeOffset.UtcNow));
            return currentTime;
        }
    }
}