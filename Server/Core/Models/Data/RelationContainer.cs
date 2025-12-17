using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Nodes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crolow.Cms.Server.Core.Models.Data
{
    [Template(Module = "Core", StorageKey = "Relations")]
    public class RelationContainer : IRelationContainer
    {
        RelationContainer()
        {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
        public bool IsField { get; set; }
        public string FieldName { get; set; }
        public int Sequence { get; set; }
        public IDataLink RelationDefinitionId { get; set; }
        public IDataLink SourceNode { get; set; }
        public IRelationLink[] TargetNodes { get; set; }

        [BsonIgnore]
        public EditState EditState { get; set; }
    }

    public interface RelationLink : IRelationLink
    {
        DataLink DataLink { get; set; }
    }
}
