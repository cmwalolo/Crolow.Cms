using Crolow.Cms.Server.Core.Models.Actions;

namespace Crolow.Cms.Server.Core.Interfaces.Application
{
    public interface IActionManager
    {
        void ProcessAction(string method, BaseRequest request);
    }
}