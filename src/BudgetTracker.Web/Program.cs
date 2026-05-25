using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Infrastructure;
using BudgetTracker.Infrastructure.Persistence;
using BudgetTracker.Web.Middleware;
using BudgetTracker.Web.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container
// NOTE: Access control is handled by ApiKeyMiddleware, so we avoid ASP.NET authorization policies here.
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

// Add infrastructure services
builder.Services.AddInfrastructure(builder.Configuration);

// Register application services
builder.Services.AddScoped<IBudgetService, BudgetService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>("database", HealthStatus.Unhealthy, tags: new[] { "db", "ready" });

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Configure security headers
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
    options.Preload = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add request logging middleware
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<RequestLoggingMiddleware>();
}

app.UseRouting();

// Custom middleware
app.UseMiddleware<ApiKeyMiddleware>();

// Configure endpoints
app.MapRazorPages();
app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(x => new
            {
                name = x.Key,
                status = x.Value.Status.ToString(),
                description = x.Value.Description,
                duration = x.Value.Duration.TotalMilliseconds
            }),
            totalDuration = report.TotalDuration.TotalMilliseconds
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.Logger.LogInformation("Starting BudgetTracker.Web application...");

app.Run();
