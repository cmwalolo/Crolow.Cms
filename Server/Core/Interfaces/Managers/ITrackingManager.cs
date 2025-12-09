using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Kalow.Apps.Models.Data;
using MongoDB.Bson;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface ITrackingManager
    {
        Tracking GetTracking(ObjectId link);
        Tracking GetTracking(IDataObject dataObject);
        void Update(ITracking node);
    }
}