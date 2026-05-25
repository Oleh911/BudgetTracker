using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BudgetTracker.Web.Pages.Budgets;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public decimal AllocatedAmount { get; set; }

    [BindProperty]
    public CurrencyCode Currency { get; set; } = CurrencyCode.UAH;

    [BindProperty]
    public string? Note { get; set; }

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var budget = new Budget(Name, AllocatedAmount, Currency, Note);
        _context.Budgets.Add(budget);
        await _context.SaveChangesAsync();

        return RedirectToPage("/Budgets/Index");
    }
}