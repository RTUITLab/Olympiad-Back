using Microsoft.AspNetCore.Http;

namespace WebApp.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetClientIP(this HttpContext httpContext)
        {
            var remoteIP = httpContext.Connection.RemoteIpAddress.ToString();
            if (httpContext.Request.Headers.TryGetValue("x-forwarded-for", out var forwardedFor))
            {
                remoteIP = forwardedFor.ToString();
            }
            else if (httpContext.Request.Headers.TryGetValue("x-real-ip", out var realIp))
            {
                remoteIP = realIp.ToString();
            }
            return remoteIP;
        }
        public static string GetClientUserAgent(this HttpContext httpContext)
        {
            return httpContext.Request.Headers.TryGetValue("user-agent", out var value) ? value.ToString() : string.Empty;
        }
    }
}
