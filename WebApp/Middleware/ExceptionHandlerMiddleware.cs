using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using WebApp.Models;

namespace WebApp.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await this._next(httpContext);
            }
            catch (StatusCodeException exception)
            {
                httpContext.Response.StatusCode = (int)exception.StatusCode;
                httpContext.Response.Headers.Clear();
            }
            catch (NotImplementedException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            }
        }
    }
    
    public static class ExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}
