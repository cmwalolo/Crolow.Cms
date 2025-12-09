using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;

namespace Kalow.Apps.Repositories.Nodes
{
    public class NodeDefinitionRepository : Repository, INodeDefinitionRepository
    {
        public NodeDefinitionRepository(MongoContext context) : base(context)
        {
        }
    }
}