using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Kalow.Apps.Models.Data;
using MongoDB.Bson;

namespace Crolow.Cms.Server.Core.Models.Data
{
    [Template(Module = "Core", StorageKey = "Data")]
    public class DataObject : IDataObject
    {
        public DataObject()
        {
            Id = ObjectId.Empty;
            Tracking = new Tracking();
            EditState = EditState.Unchanged;
        }
        public ObjectId Id { get; set; }
        public ObjectId DataStoreId { get; set; }
        public ITracking Tracking { get; set; }

        [MongoDB.Bson.Serialization.Attributes.BsonIgnore]
        public EditState EditState { get; set; }
    }
}