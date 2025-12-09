using Crolow.Apps.Common.Reflection;
using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Constants;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Extensions;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Data;
using Crolow.Cms.Server.Core.Models.Databases;
using Crolow.Cms.Server.Core.Models.Templates.Data;
using Kalow.Apps.Models.Nodes;
using MongoDB.Bson;
using System.Reflection;

namespace Kalow.Apps.Managers.Upgrades.Upgrade
{
    public class BaseUpgrade
    {
        protected readonly IManagerFactory managerFactory;
        protected IDatabaseProvider databaseProvider => managerFactory.DatabaseProvider;
        protected INodeManager nodeManager => managerFactory.NodeManager;

        public BaseUpgrade(IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        protected void MoveNewDataStores()
        {
            NodeDefinition rootNode = null;
            foreach (var store in databaseProvider.GetAll())
            {
                NodeDefinition node = nodeManager.GetNode(store);
                if (node == null)
                {
                    node = NodeDefinitionExtension.CreateNode(store,
                                        $"{store.Schema}.{store.TableName}".ToLower(),
                                        $"{store.Schema}.{store.TableName}");

                    rootNode = rootNode ?? nodeManager.EnsureFolder(store, "database/datastores");
                    node.SetParent(rootNode);
                    node.EditState = EditState.New;
                    nodeManager.Update(node);
                }

                node = nodeManager.GetNode(node.Id);
            }
        }

        protected void EnsureIndex<T>(string name, string indexes) where T : IDataObject
        {
            var repository = databaseProvider.GetContext<T>();
            repository.CreateIndex<T>(name, indexes);
        }

        protected void EnsureDataStoreObject<T>(bool doExtraTables) where T : IDataObject
        {
            databaseProvider.CreateStore<T>(doExtraTables);

            // Datastore will be created with default ID
            var repository = databaseProvider.GetContext<T>();
            repository.CreateIndex<T>("TemplateId", "TemplateId.Id");
            repository.CreateIndex<T>("TemplateIds", "TemplateIds.Id");
            var store = databaseProvider.GetStore<T>();

            if (doExtraTables)
            {
                var nodeRepository = databaseProvider.GetNodeContext();

                nodeRepository.CreateIndex<NodeDefinition>("Key", "Key");
                nodeRepository.CreateIndex<NodeDefinition>("Parent", "Parent.Id");
                nodeRepository.CreateIndex<NodeDefinition>("Parents", "Parents.Id");
                nodeRepository.CreateIndex<NodeDefinition>("TemplateId", "TemplateId.Id");
                nodeRepository.CreateIndex<NodeDefinition>("TemplateIds", "TemplateIds.Id");

                databaseProvider.GetTrackingContext();


                var relationRepository = databaseProvider.GetRelationsContext();
                relationRepository.CreateIndex<RelationContainer>("Source", "SourceNode.Id");
                relationRepository.CreateIndex<RelationContainer>("Target", "TargetNodes.Id");
            }
        }

        protected void DoTemplates(Assembly assembly)
        {
            var store = databaseProvider.GetStore<DataTemplate>();
            var dataManager = managerFactory.DataManager<DataTemplate>();

            var rootNode = nodeManager.EnsureFolder(store, "templating/templates");

            var templateTypes = ReflectionHelper.GetClassesWithAttribute(typeof(TemplateAttribute), true, assembly);
            foreach (var templateType in templateTypes)
            {
                var templateAttribute = (TemplateAttribute)templateType.GetCustomAttributes(typeof(TemplateAttribute), false).FirstOrDefault();
                if (templateAttribute != null)
                {
                    DataTemplate template = new DataTemplate();
                    template.Name = templateType.Name;
                    template.DefaultType = templateType.AssemblyQualifiedName;
                    template.DefaultNodePath = templateAttribute.NodePath;

                    if (!string.IsNullOrEmpty(templateAttribute.NodeName))
                    {
                        template.NodeNameTransformation = templateAttribute.NodeName;
                    }

                    if (templateType == typeof(DataStore))
                    {
                        template.Id = ObjectIds.DataStoreTemplateId;
                    }
                    else if (templateType == typeof(DataTemplate))
                    {
                        template.Id = ObjectIds.TemplateTemplateId;
                    }
                    else
                    {
                        template.Id = ObjectId.GenerateNewId();
                    }

                    template.EditState = EditState.New;

                    var node = NodeDefinitionExtension.CreateNode(template, template.Name.ToLower(), template.Name);
                    node.SetParent(rootNode);

                    dataManager.Update(template);
                    nodeManager.Update(node);
                }
            }
        }
    }
}