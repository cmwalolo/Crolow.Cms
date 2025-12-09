using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace Kalow.Apps.Models.Nodes
{
    [Template(Schema = "Core", StorageKey = "Nodes")]
    public class NodeDefinition : INodeDefinition
    {
        public NodeDefinition()
        {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
        public ObjectId[] Parents { get; set; }
        public ObjectId DatastoreId { get; set; }
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public int SortOrder { get; set; }
        public bool InternalNode { get; set; }
        public ObjectId Parent
        {
            get
            {
                return Parents == null ? ObjectId.Empty : Parents.Last();
            }

            set { }
        }

        [BsonIgnore]
        public EditState EditState { get; set; }

    }
}