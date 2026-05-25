using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Web.Models;

/// <summary>
/// View model for adding an expense to a budget
/// </summary>
public sealed class AddExpenseViewModel
{
    [Required]
    public Guid BudgetId { get; init; }

    public string BudgetName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Subcategory is required")]
    [Display(Name = "Subcategory")]
    public Guid SubcategoryId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Display(Name = "Amount")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Date is required")]
    [Display(Name = "Date")]
    public DateTimeOffset OccurredAt { get; set; }

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    [Display(Name = "Note")]
    public string? Note { get; set; }

    public List<SelectListItem> Subcategories { get; set; } = new();
}
