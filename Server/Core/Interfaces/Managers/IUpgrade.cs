
using Crolow.Cms.Server.Core.Models.Actions;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IUpgrade
    {
        void DoUpgrade(BaseRequest request);
        void PostUpgrade(BaseRequest request);
    }
}