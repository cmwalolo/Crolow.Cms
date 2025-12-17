using Crolow.Cms.Server.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;


namespace Crolow.Cms.Server.Core.Interfaces.Models.Data
{
    public interface ITracking : IDataMapped
    {
        [BsonIgnore]
        EditState EditState { get; set; }

        ObjectId CreatedBy { get; set; }
        ObjectId ModifiedBy { get; set; }
        ObjectId PublishedBy { get; set; }
        ObjectId LockedBy { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateModified { get; set; }
        DateTime DatePublished { get; set; }
        DateTime DateLocked { get; set; }
    }
}