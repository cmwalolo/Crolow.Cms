using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Core.Models.Databases;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IDatabaseContext
    {
        static abstract void CleanUp();
        static abstract IDatabaseContext GetContext(IOptions<DatabaseSettings> settings, DataStore store);
        IMongoCollection<T> Collection<T>(DataStore store, string table = null);
    }
}