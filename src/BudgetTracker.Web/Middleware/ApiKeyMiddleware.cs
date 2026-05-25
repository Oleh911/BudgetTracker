namespace BudgetTracker.Web.Middleware;

public sealed class ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeyCookieName = "ApiKey";

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        var method = context.Request.Method;

        logger.LogDebug("Processing {Method} request for path: {Path}", method, path);

        if (IsExemptPath(path))
        {
            logger.LogDebug("Path {Path} is exempt from authentication", path);
            await next(context);
            return;
        }

        // Перевірка чи відповідь ще не почала відправлятися
        if (context.Response.HasStarted)
        {
            logger.LogWarning("Response has already started for path: {Path}", path);
            return;
        }

        var apiKey = context.Request.Cookies[ApiKeyCookieName] 
                     ?? context.Request.Headers[ApiKeyHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogInformation("No API key found for {Method} {Path}, redirecting to /Login", method, path);
            context.Response.Redirect("/Login");
            return;
        }

        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();
        var validApiKey = configuration["Security:ApiKey"];

        if (string.IsNullOrWhiteSpace(validApiKey))
        {
            logger.LogError("Valid API key not configured in appsettings");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "Server misconfiguration" });
            return;
        }

        if (!string.Equals(validApiKey, apiKey, StringComparison.Ordinal))
        {
            logger.LogWarning("Invalid API key provided for {Method} {Path}", method, path);
            context.Response.Redirect("/Login?error=invalid");
            return;
        }

        logger.LogDebug("API key validated successfully for {Path}", path);
        await next(context);
    }

    private static bool IsExemptPath(string path)
    {
        var exemptPaths = new[]
        {
            "/login",
            "/health",
            "/css",
            "/js",
            "/lib",
            "/favicon.ico",
            "/_framework",
            "/_vs",
            "/__browserlink"
        };

        return exemptPaths.Any(exempt => path.StartsWith(exempt, StringComparison.OrdinalIgnoreCase));
    }
}