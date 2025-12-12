using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Crolow.Cms.DataLayer.Mongo
{
    public class ModuleProviderManager : IModuleProviderManager
    {
        public static object _lock = new object();

        protected static Dictionary<string, ModuleProvider> cacheByKey = new Dictionary<string, ModuleProvider>();

        protected readonly DatabaseContextManager databaseContextManager;
        protected readonly IOptions<DatabaseSettings> settings;

        public ModuleProviderManager(DatabaseContextManager databaseContextManager,
                                    IOptions<DatabaseSettings> settings)
        {
            this.databaseContextManager = databaseContextManager;
            this.settings = settings;
        }

        public IModuleProvider GetModuleProvider(string moduleName)
        {
            if (!cacheByKey.ContainsKey(moduleName))
            {
                return cacheByKey[moduleName];
            }
            else
            {
                lock (_lock)
                {
                    var provider = new ModuleProvider(databaseContextManager, settings, moduleName);
                    cacheByKey.Add(moduleName, provider);
                    return provider;
                }
            }

        }
    }
}