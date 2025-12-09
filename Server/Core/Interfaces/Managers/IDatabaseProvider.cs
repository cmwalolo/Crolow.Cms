using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Models.Databases;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IDatabaseProvider
    {
        void CreateStore<T>(bool doDefaults);

        DataStore GetNodeStore(ObjectId link);
        DataStore GetRelationStore(ObjectId link);
        DataStore GetTrackingStore(ObjectId link);
        DataStore GetStore<T>(string tableName = null);
        DataStore GetStore(ObjectId link);
        DataStore GetDataTranslationStore(ObjectId link);

        IDataRepository GetContext<T>(string schema = null);
        IDataSlipRepository GetDataSlipContext();
        IDataTranslationRepository GetDataTranslationContext(string schema = null);
        INodeDefinitionRepository GetNodeContext(string schema = null);
        IDataRelationRepository GetRelationsContext(string schema = null);
        ITrackingRepository GetTrackingContext(string schema = null);
        ITransactionRepository GetTransactionContext();
        List<DataStore> GetAll();
    }
}