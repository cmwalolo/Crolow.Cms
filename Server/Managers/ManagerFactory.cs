using Crolow.Cms.Server.Core.Interfacers.Managers;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Crolow.Cms.Server.Managers
{
    public class ManagerFactory : IManagerFactory
    {
        protected readonly IServiceProvider serviceProvider;

        public ManagerFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IModuleProvider DatabaseProvider => serviceProvider.GetService<IModuleProvider>();
        public ITemplateProvider TemplateProvider => serviceProvider.GetService<ITemplateProvider>();

        public INodeManager NodeManager => serviceProvider.GetService<INodeManager>();
        public IServiceProvider ServiceProvider => serviceProvider;

        public ITranslationManager TranslationManager => serviceProvider.GetService<ITranslationManager>();
        public IRelationManager RelationManager => serviceProvider.GetService<IRelationManager>();
        public ITrackingManager TrackingManager => serviceProvider.GetService<ITrackingManager>();

        public IDataManager<T> DataManager<T>() where T : IDataObject
        {
            return serviceProvider.GetRequiredService<IDataManager<T>>();
        }

        public IEntityManager<T> EntityManager<T>() where T : IDataObject
        {
            return serviceProvider.GetRequiredService<IEntityManager<T>>();
        }
    }
}