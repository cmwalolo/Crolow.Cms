using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Extensions;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Kalow.Apps.Models.Nodes;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Kalow.Apps.Managers.Data
{
    public class NodeManager : INodeManager
    {
        protected readonly IManagerFactory managerFactory;
        protected IDatabaseProvider databaseProvider => managerFactory.DatabaseProvider;

        public NodeManager(IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        #region Read 
        public NodeDefinition GetNode(IDataObject dataObject)
        {
            var nodeRepository = this.databaseProvider.GetNodeContext();
            return nodeRepository.Get<NodeDefinition>(t => t.Id == dataObject.Id).Result;
        }
        public NodeDefinition GetNode(ObjectId dataLink)
        {
            var nodeRepository = this.databaseProvider.GetNodeContext();
            return nodeRepository.Get<NodeDefinition>(t => t.Id == dataLink).Result;
        }

        public IEnumerable<NodeDefinition> GetChildren(ObjectId dataLink)
        {
            var nodeRepository = this.databaseProvider.GetNodeContext();
            var filter = Builders<NodeDefinition>.Filter.Eq("ParentId.Guid", dataLink);
            return nodeRepository.List<NodeDefinition>(t => t.Parent == dataLink).Result;
        }
        #endregion

        #region utils
        public NodeDefinition EnsureFolder(IDataObject dataObject, string path)
        {
            NodeDefinition node = null;
            var nodeRepository = this.databaseProvider.GetNodeContext();
            var nodeStore = this.databaseProvider.GetNodeStore(dataObject.Id);

            ObjectId parent = ObjectId.Empty;
            foreach (string p in path.Split('/'))
            {
                var parentNode = nodeRepository.Get<NodeDefinition>(t => t.Parent == parent && t.Key == p).Result;
                if (parentNode == null)
                {
                    parentNode = new NodeDefinition();
                    parentNode.Id = dataObject.Id;
                    parentNode.DatastoreId = /* TODO */ nodeStore.Id;

                    parentNode.Key = p;
                    if (node != null)
                    {
                        parentNode.SetParent(node);
                    }

                    nodeRepository.Add<NodeDefinition>(parentNode);
                }
                node = parentNode;
            }

            return node;
        }
        public NodeDefinition EnsureFolderFrom(IDataObject dataObject, string path)
        {
            NodeDefinition node = null;
            var nodeRepository = this.databaseProvider.GetNodeContext();
            var nodeStore = this.databaseProvider.GetNodeStore(dataObject.Id);

            ObjectId parent = dataObject.Id;
            foreach (string p in path.Split('/'))
            {
                var builder = Builders<NodeDefinition>.Filter;

                var parentNode = nodeRepository.Get<NodeDefinition>(t => t.Parent == parent && t.Key == p).Result;
                if (parentNode == null)
                {
                    parentNode = new NodeDefinition();
                    parentNode.Id = dataObject.Id;

                    parentNode.Key = path;
                    if (node != null)
                    {
                        parentNode.SetParent(node);
                    }

                    nodeRepository.Add<NodeDefinition>(parentNode);
                }
                node = parentNode;
            }

            return node;
        }
        #endregion

        #region Write
        public void Update(INodeDefinition node)
        {
            var nodeRepository = this.databaseProvider.GetNodeContext();
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