using Api.CommonLib.Models;

namespace Api.ChatLib.Models
{
    public class ChatMongoConfigModel : MongoConfigModel
    {
        public virtual ChatMongoConfigCollectionModel? Collections { get; set; }
    }

    public class ChatMongoConfigCollectionModel
    {
        public virtual string? Chat { get; set; }
    }
}