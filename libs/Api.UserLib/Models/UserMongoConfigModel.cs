using Api.CommonLib.Models;

namespace Api.UserLib.Models
{
    public class UserMongoConfigModel : MongoConfigModel
    {
        public virtual UserMongoConfigCollectionModel? Collections { get; set; }
    }

    public class UserMongoConfigCollectionModel
    {
        public virtual string? User { get; set; }
    }
}