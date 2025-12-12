using AutoMapper;
using Crolow.Apps.Common.Reflection;
using Crolow.Cms.DataLayer.Mongo;
using Crolow.Cms.Server.Actions;
using Crolow.Cms.Server.Actions.Actions.Upgrades;
using Crolow.Cms.Server.Common.Interfaces;
using Crolow.Cms.Server.Core.Interfacers.Managers;
using Crolow.Cms.Server.Core.Interfaces.Application;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Managers;
using Crolow.Cms.Server.Managers.Upgrades;
using Kalow.Apps.Managers.Data;
using Kalow.Apps.Managers.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crolow.Cms.Server.Api
{
    public class CoreStartup : IApplicationStartup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            return;
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<IUpgradeManager, UpgradeManager>();
            services.AddSingleton<IManagerFactory, ManagerFactory>();

            services.AddTransient<IModuleProviderManager, ModuleProviderManager>();

            services.AddTransient<ITemplateProvider, TemplateProvider>();
            services.AddTransient<INodeManager, NodeManager>();
            services.AddTransient(typeof(IDataManager<>), typeof(DataManager<>));
            services.AddTransient(typeof(IEntityManager<>), typeof(EntityManager<>));
            services.AddTransient<ITranslationManager, TranslationManager>();
            services.AddTransient<IRelationManager, RelationManager>();
            services.AddTransient<ITrackingManager, TrackingManager>();
            services.AddTransient<IActionManager, ActionManager>();

            services.AddTransient<UpgradeAction>();

            // *** TODO **** Create BsonAutomapper  for LiteDb
            //BsonAutoMapper.DefaultMappers(typeof(DataObject).Assembly);

            // Automapper => Find Profiles by reflection 
            var profiles = ReflectionHelper.GetSubclassesOf(typeof(Profile), true);
            var mapconfig = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    var resolvedProfile = System.Activator.CreateInstance(profile) as Profile;
                    cfg.AddProfile(resolvedProfile);
                }
            });

            //register mapper using config
            services.AddSingleton(mapconfig.CreateMapper());
        }

    }
}
