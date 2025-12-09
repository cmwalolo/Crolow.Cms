using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Kalow.Apps.Models.Data
{
    [Template(Schema = "Core", StorageKey = "Tracking")]
    public class Tracking : ITracking
    {
        public ObjectId Id { get; set; }
        public ObjectId CreatedBy { get; set; }
        public ObjectId ModifiedBy { get; set; }
        public ObjectId PublishedBy { get; set; }
        public ObjectId LockedBy { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateLocked { get; set; }

        [BsonIgnore]
        public EditState EditState { get; set; }
    }
}
