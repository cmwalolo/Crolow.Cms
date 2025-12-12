using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Models.Databases;

namespace Crolow.Cms.DataLayer.Mongo.Repositories
{
    public class DataRepository : Repository, IDataRepository
    {
        public DataRepository(DatabaseContextManager manager, DataStore store) : base(manager, store)
        {
        }

    }
}