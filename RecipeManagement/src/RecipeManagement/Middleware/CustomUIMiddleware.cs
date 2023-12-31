
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
        var path = context.Request.Path.Value.TrimStart('/');

        // Check if the request is for the custom UI or its assets
        if (context.Request.Path.StartsWithSegments("/custom-ui", StringComparison.OrdinalIgnoreCase) 
            || context.Request.Path.StartsWithSegments("/assets", StringComparison.OrdinalIgnoreCase))
        {
            
            if (path.StartsWith("custom-ui"))
            {
                path = path.Substring("custom-ui".Length).TrimStart('/');
            }

            // If path is empty, it's a request for the root of custom-ui
            if (string.IsNullOrEmpty(path))
            {
                path = "index.html";
            }

            var resourceName = $"RecipeManagement.CustomUI.{path.Replace("/", ".")}";

            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(resourceName);

            if (resourceStream != null)
            {
                // Determine the content type
                var contentType = GetContentType(path);
                context.Response.ContentType = contentType;

                if (path.Equals("index.html", StringComparison.OrdinalIgnoreCase))
                {
                    using var reader = new StreamReader(resourceStream);
                    var content = await reader.ReadToEndAsync();

                    // Here you inject the environment variable value into the placeholder in your index.html
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    content = content.Replace("{{ASPNETCORE_ENVIRONMENT}}", environment);

                    await context.Response.WriteAsync(content);
                    return; // Ensure you exit after handling the response
                }
                
                await resourceStream.CopyToAsync(context.Response.Body);
            }
            else
            {
                // Log the missing resource for debugging purposes
                var logger = context.RequestServices.GetService<ILogger<CustomUIMiddleware>>();
                logger?.LogWarning($"Resource not found: {resourceName}");

                // If the resource is not found, you can decide to serve a 404 page or just set the status code.
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
        }
        else
        {
            // Continue the middleware pipeline for other requests
            await _next(context);
        }
    }



// Helper method to determine content type
private string GetContentType(string path)
{
    return path switch
    {
        var p when p.EndsWith(".html", StringComparison.OrdinalIgnoreCase) => "text/html",
        var p when p.EndsWith(".js", StringComparison.OrdinalIgnoreCase) => "application/javascript",
        var p when p.EndsWith(".css", StringComparison.OrdinalIgnoreCase) => "text/css",
        var p when p.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) => "image/svg+xml",
        // Add more mappings as necessary
        _ => "application/octet-stream"
    };
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