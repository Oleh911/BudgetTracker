using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Subcategories;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty(SupportsGet = true)]
    public Guid? CategoryId { get; set; }

    public List<SubcategoryViewModel> Subcategories { get; set; } = new();
    public List<SelectListItem> Categories { get; set; } = new();

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Categories = await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Kind)
            .ThenBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Kind}: {x.Name}"
            })
            .ToListAsync();

        var query = _context.Subcategories
            .AsNoTracking()
            .Join(
                _context.Categories.AsNoTracking(),
                subcategory => subcategory.CategoryId,
                category => category.Id,
                (subcategory, category) => new { subcategory, category })
            .AsQueryable();

        if (CategoryId.HasValue)
        {
            query = query.Where(x => x.subcategory.CategoryId == CategoryId.Value);
        }

        Subcategories = await query
            .OrderBy(x => x.category.Kind)
            .ThenBy(x => x.category.Name)
            .ThenBy(x => x.subcategory.Name)
            .Select(x => new SubcategoryViewModel
            {
                Id = x.subcategory.Id,
                CategoryId = x.category.Id,
                CategoryName = x.category.Name,
                CategoryKind = x.category.Kind,
                Name = x.subcategory.Name,
                CreatedAt = x.subcategory.CreatedAt,
                UpdatedAt = x.subcategory.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(Guid id)
    {
        var subcategory = await _context.Subcategories.FirstOrDefaultAsync(x => x.Id == id);

        if (subcategory is null)
        {
            return NotFound();
        }

        _context.Subcategories.Remove(subcategory);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["ErrorMessage"] = "This subcategory is already used in operations and cannot be deleted.";
        }

        return RedirectToPage(new { categoryId = CategoryId });
    }

    public sealed class SubcategoryViewModel
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public BudgetTracker.Domain.Enums.CategoryKind CategoryKind { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}