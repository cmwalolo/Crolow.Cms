using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Extensions;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Databases;
using Crolow.Cms.Server.Core.Models.Nodes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kalow.Apps.Managers.Data
{
    public class NodeManager : INodeManager
    {
        protected IModuleProvider moduleProvider;

        public NodeManager(IModuleProvider moduleProvider)
        {
            this.moduleProvider = moduleProvider;
        }

        #region Read 
        public async Task<NodeDefinition> GetNodeAsync(IDataObject dataObject)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            return await nodeRepository.Get<NodeDefinition>(t => t.Id == dataObject.Id);
        }
        public async Task<NodeDefinition> GetNodeAsync(ObjectId dataLink)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            return await nodeRepository.Get<NodeDefinition>(t => t.Id == dataLink);
        }

        public async Task<IEnumerable<NodeDefinition>> GetChildrenAsync(ObjectId dataLink)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            var filter = Builders<NodeDefinition>.Filter.Eq("ParentId.Guid", dataLink);
            return await nodeRepository.List<NodeDefinition>(t => t.Parent == dataLink);
        }
        #endregion

        #region utils
        public async Task<NodeDefinition> EnsureFolderAsync(string path)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            NodeDefinition parentNode = new NodeDefinition();
            foreach (string p in path.Split('/'))
            {
                parentNode = await nodeRepository.Get<NodeDefinition>(t => t.Parent == parentNode.Id && t.Key == p);
                if (parentNode == null)
                {
                    var node = new NodeDefinition();
                    node.Id = ObjectId.GenerateNewId();
                    node.InternalNode = true;
                    node.DataLink =
                        new DataLink
                        {
                            DataId = ObjectId.Empty,
                            DatastoreId = ObjectId.Empty
                        };
                    node.Key = p;
                    if (parentNode != null)
                    {
                        node.SetParent(parentNode);
                    }

                    await nodeRepository.Add<NodeDefinition>(node);
                    parentNode = node;
                }
            }

            // Last node 
            return parentNode;
        }
        public async Task<NodeDefinition> EnsureFolderFromAsync(DataStore store, NodeDefinition node, string path)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            var parentNode = await nodeRepository.Get<NodeDefinition>(t => t.Parent == node.Id);

            foreach (string p in path.Split('/'))
            {
                var builder = Builders<NodeDefinition>.Filter;

                parentNode = nodeRepository.Get<NodeDefinition>(t => t.Parent == parentNode.Id && t.Key == p).Result;
                if (parentNode == null)
                {
                    var newNode = new NodeDefinition();
                    newNode.Id = ObjectId.GenerateNewId();
                    newNode.Key = path;

                    if (parentNode != null)
                    {
                        newNode.SetParent(parentNode);
                    }

                    await nodeRepository.Add<NodeDefinition>(node);
                    parentNode = node;
                }
            }

            return parentNode;
        }
        #endregion

        #region Write
        public async Task<bool> UpdateAsync(INodeDefinition node)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            switch (node.EditState)
            {
                case EditState.New:
                    await nodeRepository.Add<INodeDefinition>(node);
                    break;
                case EditState.Update:
                    await nodeRepository.Update<INodeDefinition>(t => t.Id == node.Id, node);
                    break;
                case EditState.ToDelete:
                    await nodeRepository.Remove<INodeDefinition>(t => t.Id == node.Id);
                    break;

            }
            node.EditState = EditState.Unchanged;
            return true;
        }
        #endregion
    }
}