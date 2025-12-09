using Crolow.Cms.Server.Core.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crolow.Cms.Server.Core.Interfaces.Models.Data
{
    public interface IRelationContainer : IDataMapped
    {
        [BsonIgnore]
        EditState EditState { get; set; }

        ObjectId Id { get; set; }
        bool IsField { get; set; }
        string FieldName { get; set; }
        int Sequence { get; set; }
        ObjectId RelationDefinitionId { get; set; }
        ObjectId SourceNode { get; set; }
        IRelationLink[] TargetNodes { get; set; }
    }

    public interface IRelationLink
    {
        ObjectId Id { get; set; }
        ObjectId StoreId { get; set; }
    }
}