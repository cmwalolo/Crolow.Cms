using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;

namespace Crolow.Cms.Server.Core.Models.Data
{
    [Template(Module = "Core", StorageKey = "Data")]
    public class DataObject : IDataObject
    {
        public DataObject()
        {
            Id = ObjectId.Empty;
            EditState = EditState.Unchanged;
        }
        public ObjectId Id { get; set; }


        //        [BsonIgnore]
        public EditState EditState { get; set; }
    }
}