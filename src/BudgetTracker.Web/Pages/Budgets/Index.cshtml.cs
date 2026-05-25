using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Budgets;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public List<Budget> Budgets { get; set; } = new();

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Budgets = await _context.Budgets
            .Where(b => !b.IsArchived)
            .OrderBy(b => b.DisplayOrder)
            .ThenBy(b => b.Name)
            .ToListAsync();
    }
}