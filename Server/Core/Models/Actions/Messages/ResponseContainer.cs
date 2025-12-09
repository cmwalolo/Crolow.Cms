using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Models.Actions.Messages
{

    public class ResponseContainer
    {
        public ResponseContainer()
        {
            Responses = new List<BaseResponse>();
        }

        public List<BaseResponse> Responses { get; set; }

        public void Add(BaseResponse response)
        {
            Responses.Add(response);
        }

    }
}
