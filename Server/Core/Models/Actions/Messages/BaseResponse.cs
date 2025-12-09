using System;

namespace Crolow.Cms.Server.Core.Models.Actions.Messages
{
    public class BaseResponse
    {
        public Type Type { get; set; }
        public string Message { get; set; }

        public static ErrorResponse Create<T>(string message)
        {
            return new ErrorResponse
            {
                Type = typeof(T),
                Message = message,
            };
        }
    }
}
