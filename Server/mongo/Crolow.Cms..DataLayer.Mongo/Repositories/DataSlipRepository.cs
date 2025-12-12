using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Models.Databases;

namespace Crolow.Cms.DataLayer.Mongo.Repositories
{
    public class DataSlipRepository : Repository, IDataSlipRepository
    {
        public DataSlipRepository(DatabaseContextManager manager, DataStore store) : base(manager, store)
        {
        }
    }
}