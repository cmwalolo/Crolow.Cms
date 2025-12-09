using System;

namespace Crolow.Cms.Server.Core.Models.Actions.Messages
{

    public class ErrorResponse : BaseResponse
    {
        public int ErrorCode { get; set; }
        public Exception exception { get; set; }

        public ErrorResponse()
        {

        }

        public static ErrorResponse CreateError<T>(string message, int errorCode, Exception ex)
        {
            return new ErrorResponse
            {
                Type = typeof(T),
                Message = message,
                ErrorCode = errorCode,
                exception = ex
            };
        }
    }
}
