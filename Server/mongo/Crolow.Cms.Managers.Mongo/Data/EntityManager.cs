using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Extensions;
using Crolow.Cms.Server.Core.Interfacers.Managers;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Data;
using Crolow.Cms.Server.Core.Models.Nodes;
using Kalow.Apps.Models.Data;
using System.Collections.Generic;
using System.Linq;

namespace Kalow.Apps.Managers.Data
{
    public class EntityManager<T> : IEntityManager<T> where T : IDataObject
    {
        public readonly IDataManager<T> dataManager;
        public readonly INodeManager nodeManager;
        public readonly ITranslationManager translationManager;
        public readonly IRelationManager relationManager;
        public readonly ITrackingManager trackingManager;
        public readonly IModuleProvider databaseProvider;
        public readonly ITemplateProvider templateProvider;

        public EntityManager(IManagerFactory managerFactory)
        {
            dataManager = managerFactory.DataManager<T>();
            nodeManager = managerFactory.NodeManager;
            translationManager = managerFactory.TranslationManager;
            relationManager = managerFactory.RelationManager;
            trackingManager = managerFactory.TrackingManager;
            databaseProvider = managerFactory.DatabaseProvider;
            templateProvider = managerFactory.TemplateProvider;
        }

        #region Validation
        public void EnsureData(EntityContainer<T> container)
        {
            if (container != null)
            {


            }
        }

        public EntityContainer<T> CreateEntity(INodeDefinition parent)
        {
            EntityContainer<T> entity = System.Activator.CreateInstance<EntityContainer<T>>();

            var dataStore = databaseProvider.GetStore<T>();
            entity.DataObject = System.Activator.CreateInstance<T>();
            entity.DataObject.EditState = EditState.New;

            var template = templateProvider.GetTemplate(entity.DataObject.GetType());

            entity.NodeDefinition = new NodeDefinition();
            entity.NodeDefinition.Id = entity.DataObject.Id;
            entity.NodeDefinition.EditState = EditState.New;
            entity.NodeDefinition.DatastoreId = dataStore.Id;

            entity.Tracking = new Tracking();
            entity.Tracking.Id = entity.DataObject.Id;
            entity.Tracking.EditState = EditState.New;

            if (parent == null && !string.IsNullOrEmpty(template.DefaultNodePath))
            {
                parent = nodeManager.EnsureFolder(template.DefaultNodePath);
            }

            if (parent != null)
            {
                entity.NodeDefinition.SetParent(parent);
            }
            return entity;
        }
        #endregion

        public List<IEntityContainer<T>> Children(DataRequest link)
        {
            var result = new List<IEntityContainer<T>>();
            foreach (var node in nodeManager.GetChildren(link.DataLink))
            {
                result.Add(LoadEntity(link));
            }
            return result;
        }


        public IEntityContainer<T> LoadEntity(DataRequest link)
        {
            IEntityContainer<T> container = new EntityContainer<T>();
            if (link.LoadType.HasFlag(LoadType.LoadObject))
            {
                container.DataObject = dataManager.GetNode(link.DataLink);
            }

            if (link.LoadType.HasFlag(LoadType.LoadNode))
            {
                container.NodeDefinition = nodeManager.GetNode(link.DataLink);
            }

            if (link.LoadType.HasFlag(LoadType.LoadTracking))
            {
                container.Tracking = trackingManager.GetTracking(link.DataLink);
            }

            if (link.LoadType.HasFlag(LoadType.LoadRelations))
            {
                container.Relations = relationManager.GetRelations(link.DataLink).ToList();
            }

            if (link.LoadType.HasFlag(LoadType.LoadTranslations))
            {
                container.Translations = translationManager.GetTranslations(link.DataLink, link.Language).ToList();
            }

            return container;
        }

        public void UpdateEntity(EntityContainer<T> container)
        {
            if (container.DataObject != null) dataManager.Update(container.DataObject);
            if (container.NodeDefinition != null) nodeManager.Update(container.NodeDefinition);
            if (container.Tracking != null) trackingManager.Update(container.Tracking);
            if (container.Translations != null) translationManager.Update(container.Translations);
            if (container.Relations != null) relationManager.Update(container.Relations);
        }
    }
}
