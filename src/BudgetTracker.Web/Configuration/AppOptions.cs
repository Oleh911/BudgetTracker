namespace BudgetTracker.Web.Configuration;

/// <summary>
/// Security configuration options
/// </summary>
public sealed class SecurityOptions
{
    public const string Section = "Security";

    public string ApiKey { get; set; } = string.Empty;
    public int CookieExpirationDays { get; set; } = 30;
    public bool RequireHttps { get; set; } = true;
}

/// <summary>
/// Feature flags configuration
/// </summary>
public sealed class FeaturesOptions
{
    public const string Section = "Features";

    public bool EnableRequestLogging { get; set; } = true;
    public bool EnableResponseCompression { get; set; } = true;
}

/// <summary>
/// Health checks configuration
/// </summary>
public sealed class HealthChecksOptions
{
    public const string Section = "HealthChecks";

    public bool Enabled { get; set; } = true;
    public int CacheDurationSeconds { get; set; } = 30;
}
