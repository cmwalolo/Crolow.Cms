using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Kalow.Apps.Models.Nodes;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface INodeManager
    {
        IEnumerable<NodeDefinition> GetChildren(ObjectId dataLink);
        NodeDefinition EnsureFolder(IDataObject dataObject, string path);
        NodeDefinition EnsureFolderFrom(IDataObject dataObject, string path);

        NodeDefinition GetNode(IDataObject dataObject);
        NodeDefinition GetNode(ObjectId dataLink);
        void Update(INodeDefinition node);
    }
}