using MongoDB.Bson;

namespace Crolow.Cms.Server.Core.Interfaces.Models.Data
{
    public interface IDataLink
    {
        ObjectId DataId { get; set; }
        ObjectId DatastoreId { get; set; }
    }
}