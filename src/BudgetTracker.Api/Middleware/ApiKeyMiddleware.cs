namespace BudgetTracker.Api.Middleware;

public sealed class ApiKeyMiddleware(RequestDelegate next)
{
    private const string ApiKeyHeaderName = "X-Api-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        if (IsExemptPath(path))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(ApiKeyHeaderName, out var extractedApiKey))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { error = "API key is missing." });
            return;
        }

        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var apiKey = configuration["Security:ApiKey"];

        if (string.IsNullOrWhiteSpace(apiKey) || !string.Equals(apiKey, extractedApiKey, StringComparison.Ordinal))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { error = "Invalid API key." });
            return;
        }

        await next(context);
    }

    private static bool IsExemptPath(string path)
    {
        return path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("/health", StringComparison.OrdinalIgnoreCase);
    }
}