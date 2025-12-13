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
        public NodeDefinition GetNode(IDataObject dataObject)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            return nodeRepository.Get<NodeDefinition>(t => t.Id == dataObject.Id).Result;
        }
        public NodeDefinition GetNode(ObjectId dataLink)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            return nodeRepository.Get<NodeDefinition>(t => t.Id == dataLink).Result;
        }

        public IEnumerable<NodeDefinition> GetChildren(ObjectId dataLink)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            var filter = Builders<NodeDefinition>.Filter.Eq("ParentId.Guid", dataLink);
            return nodeRepository.List<NodeDefinition>(t => t.Parent == dataLink).Result;
        }
        #endregion

        #region utils
        public NodeDefinition EnsureFolder(string path)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            NodeDefinition parentNode = null;
            ObjectId parent = ObjectId.Empty;
            foreach (string p in path.Split('/'))
            {
                parentNode = nodeRepository.Get<NodeDefinition>(t => t.Parent == parent && t.Key == p).Result;
                if (parentNode == null)
                {
                    var node = new NodeDefinition();
                    node.Id = ObjectId.GenerateNewId();
                    node.InternalNode = true;
                    node.DataId = ObjectId.Empty;
                    node.DatastoreId = ObjectId.Empty;
                    node.Key = p;
                    if (parentNode != null)
                    {
                        node.SetParent(parentNode);
                    }

                    nodeRepository.Add<NodeDefinition>(node);
                    parentNode = node;
                }
            }

            // Last node 
            return parentNode;
        }
        public NodeDefinition EnsureFolderFrom(DataStore store, IDataObject dataObject, string path)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();

            ObjectId parent = dataObject.Id;
            NodeDefinition parentNode = null;
            foreach (string p in path.Split('/'))
            {
                var builder = Builders<NodeDefinition>.Filter;

                parentNode = nodeRepository.Get<NodeDefinition>(t => t.Parent == parent && t.Key == p).Result;
                if (parentNode == null)
                {
                    var node = new NodeDefinition();
                    node.Id = ObjectId.GenerateNewId();
                    node.Key = path;
                    node.DatastoreId = store.Id;
                    node.DataId = dataObject.Id;

                    if (parentNode != null)
                    {
                        node.SetParent(parentNode);
                    }

                    nodeRepository.Add<NodeDefinition>(node);
                    parentNode = node;
                }
            }

            return parentNode;
        }
        #endregion

        #region Write
        public void Update(INodeDefinition node)
        {
            var nodeRepository = this.moduleProvider.GetNodeContext();
            switch (node.EditState)
            {
                case EditState.New:
                    nodeRepository.Add<INodeDefinition>(node);
                    break;
                case EditState.Update:
                    nodeRepository.Update<INodeDefinition>(t => t.Id == node.Id, node);
                    break;
                case EditState.ToDelete:
                    nodeRepository.Remove<INodeDefinition>(t => t.Id == node.Id);
                    break;

            }
            node.EditState = EditState.Unchanged;
        }
        #endregion
    }
}