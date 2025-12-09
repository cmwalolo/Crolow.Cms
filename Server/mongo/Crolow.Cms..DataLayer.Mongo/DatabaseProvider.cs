using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Constants;
using Crolow.Cms.Server.Core.Interfaces.Data;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Core.Models.Data;
using Crolow.Cms.Server.Core.Models.Databases;
using Kalow.Apps.DataLayer.Mongo;
using Kalow.Apps.Models.Data;
using Kalow.Apps.Models.Nodes;
using Kalow.Apps.Repositories.Nodes;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Kalow.Apps.DataLayer.MongoDb
{
    public class DatabaseProvider : IDatabaseProvider
    {

        public static object _lock = new object();

        public static Dictionary<string, DataStore> cacheByKey = new Dictionary<string, DataStore>();
        public static Dictionary<ObjectId, DataStore> cacheById = new Dictionary<ObjectId, DataStore>();

        protected readonly IOptions<DatabaseSettings> settings;
        protected readonly IManagerFactory managerFactory;

        protected INodeManager nodeManager => managerFactory.NodeManager;

        public DatabaseProvider(IOptions<DatabaseSettings> settings, IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
            this.settings = settings;
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
        public DataStore GetNodeStore(ObjectId link)
        {
            var store = GetStore(link);
            if (store != null)
            {
                return cacheByKey[$"{store.Schema}.Nodes"];
            }
            return null;
        }

        public DataStore GetRelationStore(ObjectId link)
        {
            var store = GetStore(link);
            if (store != null)
            {
                return cacheByKey[$"{store.Schema}.Relations"];
            }
            return null;
        }

        public DataStore GetDataTranslationStore(ObjectId link)
        {
            var store = GetStore(link);
            if (store != null)
            {
                return cacheByKey[$"{store.Schema}.Translations"];
            }
            return null;
        }

        public DataStore GetTrackingStore(ObjectId link)
        {
            var store = GetStore(link);
            if (store != null)
            {
                return cacheByKey[$"{store.Schema}.Trackings"];
            }
            return null;
        }

        #endregion

        #region  Get Contexts
        public ITransactionRepository GetTransactionContext()
        {
            TransactionRepository repository = null;
            if (cacheByKey.ContainsKey("Transactions.Transactions"))
            {
                var store = cacheByKey["Transactions.Transactions"];
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), store.TableName);
                repository = new TransactionRepository(context);
            }

            return repository;
        }

        public IDataSlipRepository GetDataSlipContext()
        {
            DataSlipRepository repository = null;
            if (cacheByKey.ContainsKey("Transactions.Slips"))
            {
                var store = cacheByKey["Transactions.Slips"];
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), store.TableName);
                repository = new DataSlipRepository(context);
            }

            return repository;
        }

        public IDataRelationRepository GetRelationsContext(string schema = null)
        {
            DataRelationRepository repository = null;
            var store = GetStore<RelationContainer>();
            if (store != null)
            {
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), "Relations");
                repository = new DataRelationRepository(context);
            }

            return repository;
        }

        public ITrackingRepository GetTrackingContext(string schema = null)
        {
            TrackingRepository repository = null;
            var store = GetStore<Tracking>();
            if (store != null)
            {
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), "Trackings");
                repository = new TrackingRepository(context);
            }

            return repository;
        }

        public IDataTranslationRepository GetDataTranslationContext(string schema = null)
        {
            DataTranslationRepository repository = null;
            var store = GetStore<DataTranslation>();
            if (store != null)
            {
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), "Translations");
                repository = new DataTranslationRepository(context);
            }

            return repository;
        }

        public INodeDefinitionRepository GetNodeContext(string schema = null)
        {
            NodeDefinitionRepository repository = null;
            var store = GetStore<NodeDefinition>();
            if (store != null)
            {
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), "Nodes");
                repository = new NodeDefinitionRepository(context);
            }

            return repository;
        }

        public IDataRepository GetContext<T>(string schema = null)
        {
            DataRepository repository = null;
            var store = GetStore<T>();
            if (store != null)
            {
                var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == store.Schema), store.TableName);
                repository = new DataRepository(context);
            }

            return repository;
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
                    string key = attribute.Schema + "." + (tableName ?? attribute.StorageKey);
                    if (cacheByKey.ContainsKey(key))
                    {
                        store = cacheByKey[key];
                    }
                }

                if (store == null)
                {
                    store = CreateCollection(attribute.Schema, tableName ?? attribute.StorageKey, (tableName == null && typeof(T) == typeof(DataStore)) ? ObjectIds.DataStoreId : ObjectId.Empty);
                    cacheByKey.Add($"{store.Schema}.{store.TableName}", store);
                    cacheById.Add(store.Id, store);
                }
            }
            return store;
        }
        #endregion

        public DataStore GetStore<T>(string tableName = null)
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
                    string key = attribute.Schema + "." + (tableName ?? attribute.StorageKey);
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

        protected DataStore GetOrCreateStore(string schema, string tableName = null)
        {
            DataStore store = null;
            lock (_lock)
            {
                string key = schema + "." + tableName;
                if (cacheByKey.ContainsKey(key))
                {
                    store = cacheByKey[key];
                }

                if (store == null)
                {
                    store = CreateCollection(schema, tableName, ObjectId.Empty);
                    cacheByKey.Add($"{store.Schema}.{store.TableName}", store);
                    if (!cacheById.ContainsKey(store.Id))
                    {
                        cacheById.Add(store.Id, store);
                    }
                }
            }

            return store;
        }

        protected DataStore CreateCollection(string schema, string tableName, ObjectId id)
        {
            var store = new DataStore()
            {
                Id = id.Equals(ObjectId.Empty) ? ObjectId.GenerateNewId() : id,
                Schema = schema,
                TableName = tableName,
            };


            var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == "Core"), "DataStores");
            GetDataStoreRepository().Add(store).Wait();
            return store;
        }

        protected DataRepository GetDataStoreRepository()
        {
            var context = new MongoContext(settings.Value.Values.FirstOrDefault(p => p.Schema == "Core"), "DataStores");
            return new DataRepository(context);
        }

        protected void ReloadCache()
        {
            var list = GetDataStoreRepository().GetAll<DataStore>().Result;
            foreach (var item in list)
            {
                cacheByKey.Add($"{item.Schema}.{item.TableName}", item);
                cacheById.Add(item.Id, item);
            }
        }


    }
}