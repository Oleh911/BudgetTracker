using BudgetTracker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Web.Models;

/// <summary>
/// View model for creating a new budget
/// </summary>
public class CreateBudgetViewModel
{
    [Required(ErrorMessage = "Budget name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Budget name must be between 3 and 100 characters")]
    [Display(Name = "Budget Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Allocated amount is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Allocated amount must be greater than 0")]
    [Display(Name = "Allocated Amount")]
    public decimal AllocatedAmount { get; set; }

    [Required]
    [Display(Name = "Currency")]
    public CurrencyCode Currency { get; set; } = CurrencyCode.UAH;

    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    [Display(Name = "Note")]
    public string? Note { get; set; }
}