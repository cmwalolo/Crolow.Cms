using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Templates.Data;
using Kalow.Apps.Managers.Data;
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

        protected BaseEntityManager manager;
        protected IModuleProvider moduleProvider;
        protected INodeManager nodeManager;

        public TemplateProvider(BaseEntityManager manager)
        {
            this.manager = manager;
            this.nodeManager = manager.Common.nodeManager;
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



        protected void ReloadCache()
        {
            var list = moduleProvider.GetContext<DataTemplate>().GetAll<DataTemplate>().Result;
            foreach (var item in list)
            {
                cacheByKey.Add(Type.GetType(item.DefaultType), item);
                cacheById.Add(item.Id, item);
            }
        }
    }
}
