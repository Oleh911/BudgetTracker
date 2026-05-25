using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Controllers;

public sealed class BudgetsController(IApplicationDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var budgets = await dbContext.Budgets
            .AsNoTracking()
            .Where(x => !x.IsArchived)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return View(budgets);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var budget = await dbContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (budget is null)
        {
            return NotFound();
        }

        var operations = await dbContext.BudgetOperations
            .AsNoTracking()
            .Where(x => x.BudgetId == id)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

        var model = new BudgetDetailsViewModel
        {
            Budget = budget,
            Operations = operations,
            TotalExpenses = operations
                .Where(x => x.Kind == OperationKind.Expense)
                .Sum(x => x.Amount ?? 0),
            TotalIncome = operations
                .Where(x => x.Kind == OperationKind.Income)
                .Sum(x => x.Amount ?? 0)
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> AddExpense(Guid budgetId, CancellationToken cancellationToken)
    {
        var budget = await dbContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == budgetId, cancellationToken);

        if (budget is null)
        {
            return NotFound();
        }

        var subcategories = await dbContext.Subcategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        var model = new AddExpenseViewModel
        {
            BudgetId = budgetId,
            BudgetName = budget.Name,
            OccurredAt = DateTimeOffset.UtcNow,
            Subcategories = subcategories
                .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
                .ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(AddExpenseViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            model.Subcategories = await GetSubcategorySelectListAsync(cancellationToken);
            return View(model);
        }

        var operation = BudgetOperation.CreateExpense(
            model.BudgetId,
            model.SubcategoryId,
            model.Amount,
            model.OccurredAt,
            model.Note);

        dbContext.BudgetOperations.Add(operation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return RedirectToAction(nameof(Details), new { id = model.BudgetId });
    }

    private async Task<List<SelectListItem>> GetSubcategorySelectListAsync(CancellationToken cancellationToken)
    {
        var subcategories = await dbContext.Subcategories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return subcategories
            .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
            .ToList();
    }
}