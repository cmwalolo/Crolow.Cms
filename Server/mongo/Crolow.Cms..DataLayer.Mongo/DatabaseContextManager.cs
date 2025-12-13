using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Core.Models.Databases;
using Microsoft.Extensions.Options;
using System;

namespace Crolow.Cms.DataLayer.Mongo
{
    public class DatabaseContextManager : IDisposable, Server.Core.Interfaces.Managers.IDatabaseContextManager
    {
        private bool disposedValue;
        protected readonly IOptions<DatabaseSettings> _settings;

        public DatabaseContextManager(IOptions<DatabaseSettings> settings)
        {
            _settings = settings;
        }
        public IDatabaseContext GetContext(DataStore store)
        {
            return DatabaseContext.GetContext(_settings, store);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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