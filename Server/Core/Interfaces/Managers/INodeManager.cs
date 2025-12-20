using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Databases;
using Crolow.Cms.Server.Core.Models.Nodes;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface INodeManager
    {
        Task<IEnumerable<NodeDefinition>> GetChildrenAsync(ObjectId dataLink);
        Task<NodeDefinition> EnsureFolderAsync(string path);
        Task<NodeDefinition> EnsureFolderFromAsync(DataStore store, NodeDefinition node, string path);

        Task<NodeDefinition> GetNodeAsync(IDataObject dataObject);
        Task<NodeDefinition> GetNodeAsync(ObjectId dataLink);
        Task<bool> UpdateAsync(INodeDefinition node);
    }
}