using Microsoft.AspNetCore.Http;

namespace TechNews.Common.Library.Middlewares;

public class ResponseHeaderMiddleware
{
    private readonly RequestDelegate _next;

    public ResponseHeaderMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Response.Headers.Add("Cache-Control", "no-store");
        context.Response.Headers.Add("Pragma", "no-cache");
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "deny");

        await _next(context);
    }
}
