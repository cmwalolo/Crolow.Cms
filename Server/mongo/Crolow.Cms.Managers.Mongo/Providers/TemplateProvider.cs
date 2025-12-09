using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Templates.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kalow.Apps.Managers.Providers
{
    public class TemplateProvider : ITemplateProvider
    {
        public static object _lock = new object();

        public static Dictionary<Type, DataTemplate> cacheByKey = new Dictionary<Type, DataTemplate>();
        public static Dictionary<ObjectId, DataTemplate> cacheById = new Dictionary<ObjectId, DataTemplate>();

        protected readonly IManagerFactory managerFactory;

        protected INodeManager nodeManager => managerFactory.NodeManager;
        protected IDatabaseProvider databaseProvider => managerFactory.DatabaseProvider;
        protected IEntityManager<DataTemplateSection> sectionManager => managerFactory.EntityManager<DataTemplateSection>();
        protected IEntityManager<DataTemplateField> fieldManager => managerFactory.EntityManager<DataTemplateField>();

        public TemplateProvider(IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        protected void CheckCache()
        {
            lock (_lock)
            {
                if (!cacheByKey.Any())
                {
                    ReloadCache();
                }
            }
        }

        public DataTemplate GetTemplate(Type t)
        {
            CheckCache();
            if (cacheByKey.ContainsKey(t))
            {
                return cacheByKey[t];
            }
            return null;
        }

        public DataTemplate GetTemplate(ObjectId link)
        {
            CheckCache();
            if (cacheById.ContainsKey(link))
            {
                return cacheById[link];
            }
            return null;
        }

        private void LoadSections(DataTemplate template)
        {
            var root = nodeManager.EnsureFolderFrom(template, "sections");
            var request = new DataRequest(LoadType.LoadObjectTranslated) { DataLink = root.Id };
            template.Sections = sectionManager.Children(request);
            LoadFields(template.Sections);
        }

        private void LoadFields(List<IEntityContainer<DataTemplateSection>> sections)
        {
            foreach (var section in sections)
            {
                var request = new DataRequest(LoadType.LoadObjectTranslated) { DataLink = section.DataObject.Id };
                section.DataObject.Fields = fieldManager.Children(request);
            }
        }

        protected void ReloadCache()
        {
            var list = databaseProvider.GetContext<DataTemplate>().GetAll<DataTemplate>().Result;
            foreach (var item in list)
            {
                cacheByKey.Add(Type.GetType(item.DefaultType), item);
                cacheById.Add(item.Id, item);
            }
            foreach (var item in list)
            {
                LoadSections(item);
            }
        }
    }
}
