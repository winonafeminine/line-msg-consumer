using Api.CommonLib.Models;

namespace Api.MessageLib.Models
{
    public class MessageMongoConfigModel : MongoConfigModel
    {
        public virtual MessageMongoConfigCollectionModel? Collections { get; set; }
    }

    public class MessageMongoConfigCollectionModel
    {
        public virtual string? Message { get; set; }
        public virtual string? User { get; set; }
        public virtual string? Chat { get; set; }
    }
}