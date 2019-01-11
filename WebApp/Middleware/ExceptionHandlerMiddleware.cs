using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
                await _next(httpContext);
            }
            catch (StatusCodeException exception)
            {
                httpContext.Response.StatusCode = (int)exception.StatusCode;

            }
            catch (NotImplementedException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
            }
            catch (Exception ex)
            {
                if (ex.Message?.Contains("SPA") == true)
                    httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                else
                    throw;
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
