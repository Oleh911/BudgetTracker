using System.Diagnostics;

namespace BudgetTracker.Web.Middleware;

/// <summary>
/// Middleware for logging HTTP requests and responses with execution time
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;

        try
        {
            _logger.LogInformation("Starting {Method} request to {Path}", requestMethod, requestPath);

            await _next(context);

            stopwatch.Stop();

            _logger.LogInformation(
                "Completed {Method} request to {Path} with status {StatusCode} in {ElapsedMilliseconds}ms",
                requestMethod,
                requestPath,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(ex,
                "Failed {Method} request to {Path} after {ElapsedMilliseconds}ms",
                requestMethod,
                requestPath,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
