using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Crolow.Cms.Server.Core.Interfaces.Models.Nodes
{
    public interface INodeDefinition : IDataMapped
    {
        #region Properties

        [BsonIgnore]
        EditState EditState { get; set; }

        ObjectId Id { get; set; }
        ObjectId[] Parents { get; set; }
        ObjectId Parent { get; set; }
        IDataLink DataLink { get; set; }
        string Key { get; set; }
        string DisplayName { get; set; }
        int SortOrder { get; set; }
        bool InternalNode { get; set; }

        #endregion Properties
    }
}