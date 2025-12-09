using Crolow.Cms.Server.Core.Models.Configuration;
using MongoDB.Driver;

namespace Kalow.Apps.DataLayer.Mongo
{
    public class MongoContext
    {
        private readonly IMongoDatabase database = null;
        private readonly DatabaseSetting setting = null;
        private readonly string tableName = null;

        public MongoContext(DatabaseSetting setting, string tableName)
        {
            var client = new MongoClient(setting.ConnectionString);
            if (client != null)
                database = client.GetDatabase(setting.Database);
            this.setting = setting;

            this.tableName = $"{setting.Schema}.{tableName}";

        }

        public string TableName
        {
            get { return tableName; }
        }
        public IMongoCollection<T> Collection<T>()
        {
            return database.GetCollection<T>(tableName);
        }
    }
}