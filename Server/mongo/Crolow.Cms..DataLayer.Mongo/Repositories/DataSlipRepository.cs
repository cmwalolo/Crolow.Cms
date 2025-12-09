using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;
using Kalow.Apps.Repositories.Nodes;

namespace Repositories
{
    public class DataSlipRepository : Repository, IDataSlipRepository
    {
        public DataSlipRepository(MongoContext context) : base(context)
        {
        }
    }
}