using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;
using Kalow.Apps.Repositories.Nodes;

namespace Repositories
{
    public class DataRepository : Repository, IDataRepository
    {
        public DataRepository(MongoContext context) : base(context)
        {
        }

    }
}