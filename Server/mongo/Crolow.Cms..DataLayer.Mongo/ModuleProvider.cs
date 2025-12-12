using Crolow.Cms.DataLayer.Mongo.Repositories;
using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Constants;
using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Core.Models.Databases;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace Crolow.Cms.DataLayer.Mongo
{

    public class ModuleProvider : IModuleProvider
    {

        public static object _lock = new object();

        protected Dictionary<string, DataStore> cacheByKey = new Dictionary<string, DataStore>();
        protected Dictionary<ObjectId, DataStore> cacheById = new Dictionary<ObjectId, DataStore>();

        protected readonly IOptions<DatabaseSettings> settings;
        protected readonly DatabaseContextManager databaseContextManager;
        protected readonly string moduleName;

        internal ModuleProvider(DatabaseContextManager databaseContextManager,
                                IOptions<DatabaseSettings> settings,
                                string moduleName)
        {
            this.databaseContextManager = databaseContextManager;
            this.settings = settings;
            this.moduleName = moduleName;
            if (settings != null && settings.Value != null && settings.Value.Values != null)
            {
                lock (_lock)
                {
                    if (!cacheByKey.Any())
                    {
                        ReloadCache();
                    }
                }
            }
        }

        public List<DataStore> GetAll()
        {
            return GetDataStoreRepository().GetAll<DataStore>().Result.ToList();
        }


        #region Get Stores
        public DataStore GetNodeStore()
        {
            return cacheByKey[$"{moduleName}.Nodes"];
        }

        public DataStore GetRelationStore()
        {
            return cacheByKey[$"{moduleName}.Relations"];
        }

        public DataStore GetDataTranslationStore()
        {
            return cacheByKey[$"{moduleName}.Translations"];
        }

        public DataStore GetTrackingStore()
        {
            return cacheByKey[$"{moduleName}.Trackings"];
        }

        public DataStore GetTransactionsStore()
        {
            return cacheByKey[$"{moduleName}.Transactions"];
        }

        public DataStore GetDataSlipStore()
        {
            return cacheByKey[$"{moduleName}.Slips"];
        }

        #endregion

        #region  Get Contexts
        public ITransactionRepository GetTransactionContext()
        {
            TransactionRepository repository = null;
            var store = GetTransactionsStore();
            repository = new TransactionRepository(databaseContextManager, store);
            return repository;
        }

        public IDataSlipRepository GetDataSlipContext()
        {
            DataSlipRepository repository = null;
            var store = GetDataSlipStore();
            repository = new DataSlipRepository(databaseContextManager, store);
            return repository;
        }

        public IDataRelationRepository GetRelationsContext()
        {
            var store = GetRelationStore();
            var repository = new DataRelationRepository(databaseContextManager, store);
            return repository;
        }

        public ITrackingRepository GetTrackingContext()
        {
            var store = GetTrackingStore();
            var repository = new TrackingRepository(databaseContextManager, store);
            return repository;
        }

        public IDataTranslationRepository GetDataTranslationContext()
        {
            var store = GetDataTranslationStore();
            var repository = new DataTranslationRepository(databaseContextManager, store);
            return repository;
        }

        public INodeDefinitionRepository GetNodeContext()
        {
            var store = GetNodeStore();
            return new NodeDefinitionRepository(databaseContextManager, store);
        }

        public IDataRepository GetContext<T>()
        {
            var store = GetStore<T>();
            return new DataRepository(databaseContextManager, store);
        }
        #endregion

        #region Create Store
        public void CreateStore<T>(bool doDefaults)
        {
            CreateStore<T>();
            if (doDefaults)
            {
                CreateStore<T>("Nodes");
                CreateStore<T>("Relations");
                CreateStore<T>("Trackings");
                CreateStore<T>("Translations");
            }
        }

        private DataStore CreateStore<T>(string tableName = null)
        {
            DataStore store = null;
            // Lock to avoid double Datastore to be created
            // This should only happen on some configuration startup
            // Usually after a deploy of a new version with new tables.
            // So there is no concurrency and transaction created for this.
            //
            lock (_lock)
            {
                var attribute = (TemplateAttribute)typeof(T).GetCustomAttributes(typeof(TemplateAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    string key = (tableName ?? attribute.StorageKey);
                    if (cacheByKey.ContainsKey(key))
                    {
                        store = cacheByKey[key];
                    }
                }

                if (store == null)
                {
                    store = CreateCollection(attribute.Database, tableName ?? attribute.StorageKey, tableName == null && typeof(T) == typeof(DataStore) ? ObjectIds.DataStoreId : ObjectId.Empty);
                    cacheByKey.Add($"{store.TableName}", store);
                    cacheById.Add(store.Id, store);
                }
            }
            return store;
        }
        #endregion

        public DataStore GetStore<T>()
        {
            DataStore store = null;
            // Lock to avoid double Datastore to be created
            // This should only happen on some configuration startup
            // Usually after a deploy of a new version with new tables.
            // So there is no concurrency and transaction created for this.
            //
            lock (_lock)
            {
                var attribute = (TemplateAttribute)typeof(T).GetCustomAttributes(typeof(TemplateAttribute), false).FirstOrDefault();
                if (attribute != null)
                {
                    string key = attribute.StorageKey;
                    if (cacheByKey.ContainsKey(key))
                    {
                        store = cacheByKey[key];
                    }
                }
            }
            return store;
        }

        public DataStore GetStore(ObjectId id)
        {
            return cacheById.ContainsKey(id) ? cacheById[id] : null;
        }

        protected DataStore GetOrCreateStore(string database, string tableName = null)
        {
            DataStore store = null;
            lock (_lock)
            {
                string key = moduleName + "." + tableName;
                if (cacheByKey.ContainsKey(key))
                {
                    store = cacheByKey[key];
                }

                if (store == null)
                {
                    store = CreateCollection(database, tableName, ObjectId.Empty);
                    cacheByKey.Add($"{moduleName}.{store.TableName}", store);
                    if (!cacheById.ContainsKey(store.Id))
                    {
                        cacheById.Add(store.Id, store);
                    }
                }
            }
            return store;
        }

        protected DataStore CreateCollection(string database, string tableName, ObjectId id)
        {
            var store = new DataStore()
            {
                Id = id.Equals(ObjectId.Empty) ? ObjectId.GenerateNewId() : id,
                Schema = moduleName,
                TableName = tableName,
                Database = database
            };

            GetDataStoreRepository().Add(store).Wait();
            return store;
        }

        protected DataRepository GetDataStoreRepository()
        {
            DataStore store = new DataStore
            {
                Id = ObjectIds.DataStoreId,
                Schema = "Core",
                TableName = "DataStores",
                Database = "Core"

            };
            return new DataRepository(databaseContextManager, store);
        }

        protected void ReloadCache()
        {
            var list = GetDataStoreRepository().GetAll<DataStore>().Result;
            foreach (var item in list.Where(p => p.Schema == moduleName))
            {
                cacheByKey.Add($"{item.Schema}.{item.TableName}", item);
                cacheById.Add(item.Id, item);
            }
        }


    }
}