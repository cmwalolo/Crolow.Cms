using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Extensions;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Interfaces.Models.Nodes;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Data;
using Crolow.Cms.Server.Core.Models.Nodes;
using Kalow.Apps.Models.Data;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kalow.Apps.Managers.Data
{
    public class Common : IBaseManager
    {
        public INodeManager nodeManager { get; }
        public ITranslationManager translationManager { get; }
        public IRelationManager relationManager { get; }

        public ITemplateProvider templateProvider { get; }


        public Common(IModuleProvider moduleProvider, IModuleProvider coreModuleProvider)
        {
            nodeManager = new NodeManager(moduleProvider);
            translationManager = new TranslationManager(moduleProvider);
            relationManager = new RelationManager(moduleProvider);
        }
    }


    public class BaseEntityManager : IEntityManager
    {
        public IBaseManager Common { get; set; }
        public readonly IModuleProvider moduleProvider;
        public readonly IModuleProvider coreModuleProvider;

        public BaseEntityManager(IModuleProvider moduleProvider)
        {
            this.moduleProvider = moduleProvider;
            Common = new Common(moduleProvider, coreModuleProvider);
        }

        #region Validation
        public void EnsureData<T>(EntityContainer<T> container) where T : IDataObject
        {
            if (container != null)
            {


            }
        }
        #endregion

        public EntityContainer<T> CreateEntity<T>(INodeDefinition parent) where T : IDataObject
        {
            EntityContainer<T> entity = System.Activator.CreateInstance<EntityContainer<T>>();

            var dataStore = moduleProvider.GetStore<T>();
            entity.DataObject = System.Activator.CreateInstance<T>();
            entity.DataObject.EditState = EditState.New;
            entity.DataObject.Id = ObjectId.GenerateNewId();
            entity.DataObject.DataStoreId = dataStore.Id;

            entity.NodeDefinition = new NodeDefinition();
            entity.NodeDefinition.Id = ObjectId.GenerateNewId();
            entity.NodeDefinition.DataLink = new DataLink
            {
                DataId = entity.DataObject.Id,
                DatastoreId = dataStore.Id
            };
            entity.NodeDefinition.EditState = EditState.New;

            entity.DataObject.Tracking = new Tracking();
            entity.DataObject.Tracking.EditState = EditState.New;

            if (parent != null)
            {
                entity.NodeDefinition.SetParent(parent);
            }
            return entity;
        }

        public async Task<List<IEntityContainer<T>>> Children<T>(DataRequest link) where T : IDataObject
        {
            var result = new List<IEntityContainer<T>>();
            var nodes = await Common.nodeManager.GetChildrenAsync(link.DataLink);
            foreach (var node in nodes)
            {
                result.Add(LoadEntity<T>(link, node).Result);
            }
            return result;
        }


        public async Task<IEntityContainer<T>> LoadEntity<T>(DataRequest link, INodeDefinition node) where T : IDataObject
        {
            IEntityContainer<T> container = new EntityContainer<T>();
            if (link.LoadType.HasFlag(LoadType.LoadObject))
            {
                container.DataObject = await moduleProvider.GetContext<T>().Get<T>(node.DataLink.DataId);
            }

            if (link.LoadType.HasFlag(LoadType.LoadNode))
            {
                container.NodeDefinition = await Common.nodeManager.GetNodeAsync(node.DataLink.DataId);
            }

            if (link.LoadType.HasFlag(LoadType.LoadRelations))
            {
                container.Relations = Common.relationManager.GetRelations(node.DataLink.DataId).ToList();
            }

            if (link.LoadType.HasFlag(LoadType.LoadTranslations))
            {
                container.Translations = Common.translationManager.GetTranslations(node.DataLink.DataId, link.Language).ToList();
            }

            return container;
        }

        public void UpdateEntity<T>(EntityContainer<T> container) where T : IDataObject
        {
            if (container.DataObject != null) moduleProvider.GetContext<T>().Update<T>(p => p.Id == container.DataObject.Id, container.DataObject);
            if (container.NodeDefinition != null) Common.nodeManager.UpdateAsync(container.NodeDefinition);
            if (container.Translations != null) Common.translationManager.Update(container.Translations);
            if (container.Relations != null) Common.relationManager.Update(container.Relations);
        }
    }
}
