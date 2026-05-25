using BudgetTracker.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTracker.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IBudgetService _budgetService;
    private readonly ILogger<IndexModel> _logger;

    public int BudgetsCount { get; set; }
    public int OperationsCount { get; set; }

    public IndexModel(IBudgetService budgetService, ILogger<IndexModel> logger)
    {
        _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var (budgetsCount, operationsCount) = await _budgetService.GetStatisticsAsync(cancellationToken);
            BudgetsCount = budgetsCount;
            OperationsCount = operationsCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching statistics");
            BudgetsCount = 0;
            OperationsCount = 0;
        }
    }
}