namespace TechNews.Auth.Api.Middlewares;

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

        await _next(context);
    }
}