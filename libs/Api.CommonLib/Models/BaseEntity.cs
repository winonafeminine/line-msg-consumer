using Api.CommonLib.Services;
using Newtonsoft.Json;

namespace Api.CommonLib.Models
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            DateTimeOffset currentTime = TimezoneService.Convert();
            CreatedDate = currentTime.DateTime;
            ModifiedDate = currentTime.DateTime;
        }
        [JsonProperty("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("modified_date")]
        public DateTime ModifiedDate { get; set; }
    }
}