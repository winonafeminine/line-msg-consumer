using Api.LmcLib.Services;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.LmcLib.Models
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
        [BsonElement("created_date")]
        public DateTime CreatedDate { get; set; }

        [JsonProperty("modified_date")]
        [BsonElement("modified_date")]
        public DateTime ModifiedDate { get; set; }
    }
}