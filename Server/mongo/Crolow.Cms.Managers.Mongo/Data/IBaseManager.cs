using Crolow.Cms.Server.Core.Interfaces.Managers;

namespace Kalow.Apps.Managers.Data
{
    public interface IBaseManager
    {
        INodeManager nodeManager { get; }
        IRelationManager relationManager { get; }
        ITrackingManager trackingManager { get; }
        ITranslationManager translationManager { get; }
    }
}