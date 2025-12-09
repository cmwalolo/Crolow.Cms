using Crolow.Cms.Server.Core.Models.Actions;

namespace Crolow.Cms.Server.Core.Interfaces.Application
{
    public interface IAction
    {
        void Process(BaseRequest request);
    }
}