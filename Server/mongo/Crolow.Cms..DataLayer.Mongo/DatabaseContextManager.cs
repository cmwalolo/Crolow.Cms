using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Core.Models.Databases;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crolow.Cms.DataLayer.Mongo
{
    public class DatabaseContextManager : IDisposable, Server.Core.Interfaces.Managers.IDatabaseContextManager
    {
        private static object _lock = new object();
        private bool disposedValue;
        protected readonly IOptions<DatabaseSettings> _settings;
        private static Dictionary<string, IMongoDatabase> databases = new Dictionary<string, IMongoDatabase>();


        public DatabaseContextManager(IOptions<DatabaseSettings> settings)
        {
            _settings = settings;
        }
        public IDatabaseContext GetContext(DataStore store)
        {
            var setting = _settings.Value.Values.FirstOrDefault(s => s.Database == store.Database);
            string db = $"{setting.ClientName}.{store.Database}";

            if (databases.ContainsKey(db))
            {
                return DatabaseContext.GetContext(_settings, store);
            }
            else
            {
                lock (_lock)
                {
                    var context = DatabaseContext.GetContext(_settings, store);
                    databases.Add(db, ((DatabaseContext)context).Collection<object>(store).Database);
                }
            }

            return DatabaseContext.GetContext(_settings, store);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    databases.Clear();
                    DatabaseContext.CleanUp();
                    disposedValue = true;
                }
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}