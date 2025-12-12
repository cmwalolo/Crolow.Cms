using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Application;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Actions;

namespace Crolow.Cms.Server.Actions.Actions.Upgrades
{

    [ActionComponent(Path = "kalow/core/upgrade")]
    public class UpgradeAction : IAction
    {
        protected readonly IUpgradeManager upgradeManager;
        public UpgradeAction(IUpgradeManager upgradeManager)
        {
            this.upgradeManager = upgradeManager;
        }

        public void Process(BaseRequest request)
        {
            upgradeManager.DoUpgrade(request);
        }
    }
}
