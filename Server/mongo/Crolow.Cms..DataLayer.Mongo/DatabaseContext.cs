using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Core.Models.Databases;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace Crolow.Cms.DataLayer.Mongo
{
    public class DatabaseContext : IDatabaseContext
    {
        protected IMongoDatabase database = null;
        protected string tableName = null;

        private static Dictionary<string, MongoClient> clients = new Dictionary<string, MongoClient>();
        private static Dictionary<string, IMongoDatabase> databases = new Dictionary<string, IMongoDatabase>();

        public static void CleanUp()
        {
            foreach (var client in clients.Values)
            {
                client.Dispose();
            }

            clients = new Dictionary<string, MongoClient>();
            databases = new Dictionary<string, IMongoDatabase>();
        }

        public static IDatabaseContext GetContext(IOptions<DatabaseSettings> settings, DataStore store)
        {
            DatabaseContext ctxt = new DatabaseContext();

            MongoClient client = null;

            var setting = settings.Value.Values.FirstOrDefault(s => s.Database == store.Database);

            if (clients.ContainsKey(setting.ClientName))
            {
                client = clients[setting.ClientName];
            }
            else
            {
                client = new MongoClient(setting.ConnectionString);
                clients.Add(setting.ClientName, client);
            }


            string db = $"{setting.ClientName}.{store.Database}";
            if (databases.ContainsKey(db))
            {
                ctxt.database = databases[db];
            }
            else
            {
                ctxt.database = client.GetDatabase(store.Database);
                databases.Add(db, ctxt.database);
            }

            ctxt.database = client.GetDatabase(store.Database);
            return ctxt;
        }

        public IMongoCollection<T> Collection<T>(DataStore store, string table = null)
        {
            var tableName = $"{store.Schema}.{table ?? store.TableName}";
            return database.GetCollection<T>(tableName);
        }
    }
}