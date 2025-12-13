using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Databases;

namespace Crolow.Cms.DataLayer.Mongo.Repositories
{
    public class NodeDefinitionRepository : Repository, INodeDefinitionRepository
    {
        public NodeDefinitionRepository(IDatabaseContextManager manager, DataStore store) : base(manager, store)
        {
        }
    }
}