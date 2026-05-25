using BudgetTracker.Web.Models;
using BudgetTracker.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Controllers;

/// <summary>
/// Controller for managing budgets
/// </summary>
public sealed class BudgetsController : Controller
{
    private readonly IBudgetService _budgetService;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IBudgetService budgetService, ILogger<BudgetsController> logger)
    {
        _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var budgets = await _budgetService.GetAllBudgetsAsync(cancellationToken);
            return View(budgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching budgets");
            return View("Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var budgetDetails = await _budgetService.GetBudgetDetailsAsync(id, cancellationToken);

            if (budgetDetails is null)
            {
                return NotFound();
            }

            // Map to existing ViewModel for compatibility with existing views
            var model = new BudgetDetailsViewModel
            {
                Budget = budgetDetails.Budget.Name != null
                    ? new Domain.Entities.Budget(
                        budgetDetails.Budget.Name,
                        budgetDetails.Budget.AllocatedAmount,
                        budgetDetails.Budget.Currency,
                        budgetDetails.Budget.Note)
                    {
                        // Set private properties via reflection or use domain methods
                    }
                    : null!,
                Operations = budgetDetails.Operations.Select(o => 
                    Domain.Entities.BudgetOperation.CreateExpense(
                        o.BudgetId, 
                        o.SubcategoryId ?? Guid.Empty, 
                        o.Amount ?? 0, 
                        o.OccurredAt, 
                        o.Note)).ToList(),
                TotalExpenses = budgetDetails.TotalExpenses,
                TotalIncome = budgetDetails.TotalIncome
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching budget details for {BudgetId}", id);
            return View("Error");
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBudgetViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _budgetService.CreateBudgetAsync(model, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            ModelState.AddModelError(string.Empty, "An error occurred while creating the budget.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> AddExpense(Guid budgetId, CancellationToken cancellationToken)
    {
        try
        {
            var budgetDetails = await _budgetService.GetBudgetDetailsAsync(budgetId, cancellationToken);

            if (budgetDetails is null)
            {
                return NotFound();
            }

            var model = new AddExpenseViewModel
            {
                BudgetId = budgetId,
                BudgetName = budgetDetails.Budget.Name,
                OccurredAt = DateTimeOffset.UtcNow,
                Subcategories = new List<SelectListItem>() // TODO: Load from subcategory service
            };

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading expense form for budget {BudgetId}", budgetId);
            return View("Error");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(AddExpenseViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            // TODO: Reload subcategories
            return View(model);
        }

        try
        {
            await _budgetService.AddExpenseAsync(model, cancellationToken);
            return RedirectToAction(nameof(Details), new { id = model.BudgetId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding expense to budget {BudgetId}", model.BudgetId);
            ModelState.AddModelError(string.Empty, "An error occurred while adding the expense.");
            return View(model);
        }
    }
}