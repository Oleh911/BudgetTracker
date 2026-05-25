using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Web.Pages;

/// <summary>
/// Login page model for API key authentication
/// </summary>
[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    [Required(ErrorMessage = "API Key is required")]
    [Display(Name = "API Key")]
    public string? ApiKey { get; set; }

    public string? ErrorMessage { get; set; }

    public LoginModel(IConfiguration configuration, ILogger<LoginModel> logger)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnGet([FromQuery] string? error)
    {
        _logger.LogInformation("Login page accessed");

        if (error == "invalid")
        {
            ErrorMessage = "Invalid API Key. Please try again.";
        }
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("Login attempt received");

        if (!ModelState.IsValid)
        {
            ErrorMessage = "Please provide a valid API Key.";
            return Page();
        }

        var normalizedApiKey = ApiKey?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedApiKey))
        {
            _logger.LogWarning("Empty API key provided");
            ErrorMessage = "API Key is required.";
            return Page();
        }

        var validApiKey = _configuration["Security:ApiKey"]?.Trim();

        if (string.IsNullOrWhiteSpace(validApiKey))
        {
            _logger.LogError("API key not configured in application settings");
            ErrorMessage = "Server configuration error. Please contact administrator.";
            return Page();
        }

        if (!string.Equals(validApiKey, normalizedApiKey, StringComparison.Ordinal))
        {
            _logger.LogWarning("Invalid API key attempt");
            ErrorMessage = "Invalid API Key.";
            return Page();
        }

        _logger.LogInformation("Valid API key provided, setting authentication cookie");

        Response.Cookies.Append("ApiKey", normalizedApiKey, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return RedirectToPage("/Index");
    }

    public IActionResult OnPostLogout()
    {
        _logger.LogInformation("User logging out");
        Response.Cookies.Delete("ApiKey");
        return RedirectToPage("/Login");
    }
}
