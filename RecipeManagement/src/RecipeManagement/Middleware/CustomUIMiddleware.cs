namespace RecipeManagement.Middleware;

using System.Reflection;
using Microsoft.Net.Http.Headers;

public class CustomUIMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _embeddedFileNamespace;

    public CustomUIMiddleware(RequestDelegate next, string embeddedFileNamespace)
    {
        _next = next;
        _embeddedFileNamespace = embeddedFileNamespace;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/custom-ui", StringComparison.OrdinalIgnoreCase))
        {
            var resourceName = _embeddedFileNamespace + ".index.html";
            var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (resourceStream == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            context.Response.ContentType = "text/html";
            context.Response.Headers[HeaderNames.CacheControl] = "no-cache, no-store";
            context.Response.Headers[HeaderNames.Pragma] = "no-cache";
            context.Response.Headers[HeaderNames.Expires] = "-1";

            await resourceStream.CopyToAsync(context.Response.Body);
        }
        else
        {
            await _next(context);
        }
    }
}

// Extension method for adding the middleware
public static class CustomUIMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomUI(this IApplicationBuilder app)
    {
        var embeddedFileNamespace = "RecipeManagement.CustomUI";
        return app.UseMiddleware<CustomUIMiddleware>(embeddedFileNamespace);
    }
}