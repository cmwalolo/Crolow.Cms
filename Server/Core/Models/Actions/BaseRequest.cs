using Crolow.Cms.Server.Core.Models.Actions.Messages;

namespace Crolow.Cms.Server.Core.Models.Actions
{
    public class BaseRequest
    {
        public BaseRequest()
        {
            CancelRequest = false;
            Response = new ResponseContainer();
        }

        public bool CancelRequest { get; set; }
        public ResponseContainer Response { get; set; }
    }
}
