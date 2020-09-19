using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class StatusCodeException : Exception
    {
        public StatusCodeException(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }

        public StatusCodeException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public HttpStatusCode StatusCode { get; set; }

        public static StatusCodeException NotFount => new StatusCodeException(HttpStatusCode.NotFound);
        public static StatusCodeException TooManyRequests => new StatusCodeException(HttpStatusCode.TooManyRequests);

        public static StatusCodeException BadRequest() => new StatusCodeException(HttpStatusCode.BadRequest);
        public static StatusCodeException BadRequest(string message) => new StatusCodeException(HttpStatusCode.BadRequest, message);
        public static StatusCodeException Conflict(string message) => new StatusCodeException(HttpStatusCode.Conflict, message);
    }
}
