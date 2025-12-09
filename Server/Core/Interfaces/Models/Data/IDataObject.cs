using Crolow.Cms.Server.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crolow.Cms.Server.Core.Interfaces.Models.Data
{
    public interface IDataObject : IDataMapped
    {

        [BsonIgnore]
        EditState EditState { get; set; }
        ObjectId Id { get; set; }
    }
}