using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;

namespace Kalow.Apps.Repositories.Nodes
{
    public class TrackingRepository : Repository, ITrackingRepository
    {
        public TrackingRepository(MongoContext context) : base(context)
        {
        }
    }
}