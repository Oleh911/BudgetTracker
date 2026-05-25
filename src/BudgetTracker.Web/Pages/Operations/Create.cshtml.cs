using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Operations;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty(SupportsGet = true)]
    public Guid? BudgetId { get; set; }

    [BindProperty]
    public OperationKind Kind { get; set; } = OperationKind.Expense;

    [BindProperty]
    public Guid SelectedBudgetId { get; set; }

    [BindProperty]
    public Guid SelectedSubcategoryId { get; set; }

    [BindProperty]
    public decimal Amount { get; set; }

    [BindProperty]
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

    [BindProperty]
    public string? Note { get; set; }

    public List<SelectListItem> Budgets { get; set; } = new();
    public List<SelectListItem> Subcategories { get; set; } = new();

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();

        if (BudgetId.HasValue)
        {
            SelectedBudgetId = BudgetId.Value;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectListsAsync();
            return Page();
        }

        BudgetOperation operation = Kind switch
        {
            OperationKind.Expense => BudgetOperation.CreateExpense(
                SelectedBudgetId, SelectedSubcategoryId, Amount, OccurredAt, Note),
            OperationKind.Income => BudgetOperation.CreateIncome(
                SelectedBudgetId, SelectedSubcategoryId, Amount, OccurredAt, Note),
            _ => throw new InvalidOperationException($"Unsupported operation kind: {Kind}")
        };

        _context.BudgetOperations.Add(operation);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Operations/Index", new { budgetId = SelectedBudgetId });
    }

    private async Task LoadSelectListsAsync()
    {
        Budgets = await _context.Budgets
            .Where(b => !b.IsArchived)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = $"{b.Name} ({b.AllocatedAmount} {b.Currency})"
            })
            .ToListAsync();

        Subcategories = await _context.Subcategories
            .Join(
                _context.Categories,
                s => s.CategoryId,
                c => c.Id,
                (s, c) => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = $"{c.Name} → {s.Name}"
                })
            .ToListAsync();
    }
}