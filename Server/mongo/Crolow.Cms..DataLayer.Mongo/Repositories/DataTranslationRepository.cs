using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;

namespace Kalow.Apps.Repositories.Nodes
{
    public class DataTranslationRepository : Repository, IDataTranslationRepository
    {
        public DataTranslationRepository(MongoContext context) : base(context)
        {
        }
    }
}