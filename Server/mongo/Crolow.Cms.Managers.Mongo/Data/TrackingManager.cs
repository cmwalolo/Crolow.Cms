using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Kalow.Apps.Models.Data;
using MongoDB.Bson;

namespace Kalow.Apps.Managers.Data
{
    public class TrackingManager : ITrackingManager
    {
        protected readonly IManagerFactory managerFactory;
        protected IModuleProvider databaseProvider => managerFactory.DatabaseProvider;

        public TrackingManager(IManagerFactory managerFactory)
        {
            this.managerFactory = managerFactory;
        }

        public Tracking GetTracking(IDataObject dataObject)
        {
            var repository = this.databaseProvider.GetTrackingContext();
            return repository.Get<Tracking>(t => t.Id == dataObject.Id).Result;
        }

        public Tracking GetTracking(ObjectId link)
        {
            var repository = this.databaseProvider.GetTrackingContext();
            return repository.Get<Tracking>(t => t.Id == link).Result;
        }

        public void Update(ITracking node)
        {
            var repository = this.databaseProvider.GetTrackingContext();
            switch (node.EditState)
            {
                case EditState.New:
                    repository.Add<ITracking>(node);
                    break;
                case EditState.Update:
                    repository.Update<ITracking>(t => t.Id == node.Id, node);
                    break;
                case EditState.ToDelete:
                    repository.Remove<ITracking>(t => t.Id == node.Id);
                    break;

            }
            node.EditState = EditState.Unchanged;
        }
    }
}