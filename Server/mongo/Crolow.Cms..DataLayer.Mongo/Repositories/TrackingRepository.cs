using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Databases;

namespace Crolow.Cms.DataLayer.Mongo.Repositories
{
    public class TrackingRepository : Repository, ITrackingRepository
    {
        public TrackingRepository(IDatabaseContextManager manager, DataStore store) : base(manager, store)
        {
        }
    }
}