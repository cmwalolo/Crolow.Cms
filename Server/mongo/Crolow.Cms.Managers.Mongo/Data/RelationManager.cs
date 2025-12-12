using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Kalow.Apps.Managers.Data
{
    public class RelationManager : IRelationManager
    {
        protected readonly IManagerFactory managerFactory;
        protected IModuleProvider databaseProvider => managerFactory.DatabaseProvider;

        public RelationManager(IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        public IEnumerable<IRelationContainer> GetRelations(IDataObject dataObject)
        {
            var repository = this.databaseProvider.GetRelationsContext();
            return repository.List<IRelationContainer>(t => t.SourceNode == dataObject.Id && t.IsField).Result;
        }

        public IEnumerable<IRelationContainer> GetRelations(ObjectId dataObject)
        {
            var repository = this.databaseProvider.GetRelationsContext();
            return repository.List<IRelationContainer>(t => t.SourceNode == dataObject && t.IsField).Result;
        }
        public void Update(IRelationContainer node)
        {
            var repository = this.databaseProvider.GetRelationsContext();
            switch (node.EditState)
            {
                case EditState.New:
                    repository.Add<IRelationContainer>(node);
                    break;
                case EditState.Update:
                    repository.Update<IRelationContainer>(t => t.Id == node.Id, node);
                    break;
                case EditState.ToDelete:
                    repository.Remove<IRelationContainer>(t => t.Id == node.Id);
                    break;

            }
            node.EditState = EditState.Unchanged;
        }

        public void Update(List<IRelationContainer> node)
        {
            node.ForEach(p => Update(node));
        }
    }
}