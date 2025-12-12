using Crolow.Cms.Server.Core.Interfacers.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using System;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IManagerFactory
    {
        IModuleProvider DatabaseProvider { get; }
        ITemplateProvider TemplateProvider { get; }
        INodeManager NodeManager { get; }
        ITranslationManager TranslationManager { get; }
        IRelationManager RelationManager { get; }
        ITrackingManager TrackingManager { get; }
        IServiceProvider ServiceProvider { get; }
        IDataManager<T> DataManager<T>() where T : IDataObject;
        IEntityManager<T> EntityManager<T>() where T : IDataObject;
    }
}