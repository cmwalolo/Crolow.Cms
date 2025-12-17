using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace Crolow.Cms.Server.Core.Models.Nodes
{
    public class DataLink : IDataLink
    {
        public ObjectId DataId { get; set; }
        public ObjectId DatastoreId { get; set; }
    }


    [Template(Module = "Core", StorageKey = "Nodes")]
    public class NodeDefinition : INodeDefinition
    {
        public NodeDefinition()
        {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
        public ObjectId[] Parents { get; set; }

        public IDataLink DataLink { get; set; }

        public string Icon { get; set; }
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