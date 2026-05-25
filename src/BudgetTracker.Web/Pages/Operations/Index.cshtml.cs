using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Operations;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty(SupportsGet = true)]
    public Guid? BudgetId { get; set; }

    public List<OperationViewModel> Operations { get; set; } = new();
    public string? BudgetName { get; set; }

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        var query = _context.BudgetOperations.AsQueryable();

        if (BudgetId.HasValue)
        {
            var budget = await _context.Budgets.FindAsync(BudgetId.Value);
            BudgetName = budget?.Name;

            query = query.Where(o => o.BudgetId == BudgetId.Value);
        }

        // Replace the problematic projection with explicit joins to get category and subcategory names
        Operations = await query
            .Join(_context.Subcategories,
                o => o.SubcategoryId,
                s => s.Id,
                (o, s) => new { Operation = o, Subcategory = s })
            .Join(_context.Categories,
                os => os.Subcategory.CategoryId,
                c => c.Id,
                (os, c) => new { os.Operation, os.Subcategory, Category = c })
            .OrderByDescending(x => x.Operation.OccurredAt)
            .Select(x => new OperationViewModel
            {
                Id = x.Operation.Id,
                Kind = x.Operation.Kind,
                Amount = x.Operation.Amount ?? 0,
                CategoryName = x.Category.Name,
                SubcategoryName = x.Subcategory.Name,
                Note = x.Operation.Note,
                OccurredAt = x.Operation.OccurredAt
            })
            .ToListAsync();
    }

    public class OperationViewModel
    {
        public Guid Id { get; set; }
        public BudgetTracker.Domain.Enums.OperationKind Kind { get; set; }
        public decimal Amount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string SubcategoryName { get; set; } = string.Empty;
        public string? Note { get; set; }
        public DateTimeOffset OccurredAt { get; set; }
    }
}