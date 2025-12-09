using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;
using Kalow.Apps.Repositories.Nodes;

namespace Repositories;

public class DataRelationRepository : Repository, IDataRelationRepository
{
    public DataRelationRepository(MongoContext context) : base(context)
    {
    }
}