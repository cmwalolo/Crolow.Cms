using Crolow.Cms.Server.Core.Models.Actions;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IUpgradeManager
    {
        void DoUpgrade(BaseRequest request);
    }
}