using Crolow.Cms.Server.Core.Interfaces.Data;
using Kalow.Apps.DataLayer.Mongo;

namespace Kalow.Apps.Repositories.Nodes
{
    public class TransactionRepository : Repository, ITransactionRepository
    {
        public TransactionRepository(MongoContext context) : base(context)
        {
        }
    }
}