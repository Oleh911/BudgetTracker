using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty(SupportsGet = true)]
    public bool IncludeArchived { get; set; }

    public List<Category> Categories { get; set; } = new();

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        var query = _context.Categories.AsNoTracking().AsQueryable();

        if (!IncludeArchived)
        {
            query = query.Where(x => !x.IsArchived);
        }

        Categories = await query
            .OrderBy(x => x.Kind)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostArchiveAsync(Guid id)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        category.Archive();
        await _context.SaveChangesAsync();

        return RedirectToPage(new { includeArchived = IncludeArchived });
    }
}