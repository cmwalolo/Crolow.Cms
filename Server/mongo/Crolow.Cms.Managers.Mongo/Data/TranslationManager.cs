using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Kalow.Apps.Managers.Data
{
    public class TranslationManager : ITranslationManager
    {
        protected readonly IManagerFactory managerFactory;
        protected IModuleProvider databaseProvider => managerFactory.DatabaseProvider;

        public TranslationManager(IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        public IEnumerable<IDataTranslation> GetTranslations(IDataObject dataObject, string language)
        {
            var repository = this.databaseProvider.GetRelationsContext();
            return repository.List<IDataTranslation>(p => p.Id == dataObject.Id && p.Language == language).Result;
        }

        public IEnumerable<IDataTranslation> GetTranslations(ObjectId link, string language)
        {
            var repository = this.databaseProvider.GetTrackingContext();
            return repository.List<IDataTranslation>(p => p.Id == link && p.Language == language).Result;
        }

        public IEnumerable<IDataTranslation> GetAllTranslations(IDataObject dataObject)
        {
            var repository = this.databaseProvider.GetRelationsContext();
            return repository.List<IDataTranslation>(f => f.Id == dataObject.Id).Result;
        }

        public IEnumerable<IDataTranslation> GetAllTranslations(ObjectId link)
        {
            var repository = this.databaseProvider.GetTrackingContext();
            return repository.List<IDataTranslation>(f => f.Id == link).Result;
        }

        public void Update(IDataTranslation node)
        {
            var repository = this.databaseProvider.GetDataTranslationContext();
            switch (node.EditState)
            {
                case EditState.New:
                    repository.Add<IDataTranslation>(node);
                    break;
                case EditState.Update:
                    repository.Update<IDataTranslation>(t => t.Id == node.Id, node);
                    break;
                case EditState.ToDelete:
                    repository.Remove<IDataTranslation>(t => t.Id == node.Id);
                    break;

            }

            node.EditState = EditState.Unchanged;
        }

        public void Update(List<IDataTranslation> node)
        {
            node.ForEach(p => Update(node));
        }
    }
}