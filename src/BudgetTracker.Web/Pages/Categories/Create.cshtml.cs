using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Pages.Categories;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public string Name { get; set; } = string.Empty;

    [BindProperty]
    public CategoryKind Kind { get; set; } = CategoryKind.Expense;

    [BindProperty]
    public string? Color { get; set; }

    [BindProperty]
    public string? Icon { get; set; }

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet()
    {
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

        var category = new Category(Name, Kind, Color, Icon);
        _context.Categories.Add(category);

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