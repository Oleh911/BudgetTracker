using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Categories;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public CategoryKind Kind { get; set; } = CategoryKind.Expense;

    [BindProperty]
    public string? Color { get; set; }

    [BindProperty]
    public string? Icon { get; set; }

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        Id = category.Id;
        Name = category.Name;
        Kind = category.Kind;
        Color = category.Color;
        Icon = category.Icon;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            ModelState.AddModelError(nameof(Name), "Name is required.");
        }

        if (!Enum.IsDefined(Kind))
        {
            ModelState.AddModelError(nameof(Kind), "Category kind is invalid.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == Id);

        if (category is null)
        {
            return NotFound();
        }

        category.Update(Name, Kind, Color, Icon);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(string.Empty, "A category with the same name and kind already exists.");
            return Page();
        }

        return RedirectToPage("/Categories/Index");
    }
}