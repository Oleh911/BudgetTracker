using BudgetTracker.Domain.Entities;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Subcategories;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public Guid CategoryId { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    public List<SelectListItem> Categories { get; set; } = new();

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        await LoadCategoriesAsync();

        var subcategory = await _context.Subcategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (subcategory is null)
        {
            return NotFound();
        }

        Id = subcategory.Id;
        CategoryId = subcategory.CategoryId;
        Name = subcategory.Name;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadCategoriesAsync();

        if (CategoryId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(CategoryId), "Category is required.");
        }
        else if (!await _context.Categories.AnyAsync(x => x.Id == CategoryId))
        {
            ModelState.AddModelError(nameof(CategoryId), "Category was not found.");
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError(nameof(Name), "Name is required.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var subcategory = await _context.Subcategories.FirstOrDefaultAsync(x => x.Id == Id);

        if (subcategory is null)
        {
            return NotFound();
        }

        subcategory.Update(CategoryId, Name);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "A subcategory with the same name already exists in this category.");
            return Page();
        }

        return RedirectToPage("/Subcategories/Index", new { categoryId = CategoryId });
    }

    private async Task LoadCategoriesAsync()
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
    }
}