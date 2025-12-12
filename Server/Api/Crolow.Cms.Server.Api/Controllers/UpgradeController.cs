using Crolow.Cms.Server.Common.Configuration;
using Crolow.Cms.Server.Core.Interfaces.Application;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace Crolow.Cms.Server.Api.Controllers
{
    [Route("api/crolow/[controller]")]
    // [Authorize(Policy = "admin")]
    public class UpgradeController : Controller
    {
        protected readonly IWritableOptions<UpgradeSettings> upgradeSettings;
        protected readonly IUpgradeManager upgradeManager;

        protected readonly IActionManager actionManager;

        public UpgradeController(IActionManager actionManager)
        {
            this.actionManager = actionManager;
        }

        [HttpGet]
        public bool Upgrade()
        {
            actionManager.ProcessAction("kalow/core/upgrade", new Core.Models.Actions.BaseRequest());
            return true;
        }

    }
}
