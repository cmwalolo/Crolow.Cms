using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Models.Databases;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IModuleProvider
    {
        void ClearCache();
        void CreateStore<T>(bool doDefaults);

        DataStore GetNodeStore();
        DataStore GetRelationStore();
        DataStore GetTrackingStore();
        DataStore GetStore<T>();
        DataStore GetStore(ObjectId link);
        DataStore GetDataTranslationStore();

        IDataRepository GetContext<T>();
        IDataTranslationRepository GetDataTranslationContext();
        INodeDefinitionRepository GetNodeContext();
        IDataRelationRepository GetRelationsContext();
        ITrackingRepository GetTrackingContext();
        List<DataStore> GetAll();
    }
}