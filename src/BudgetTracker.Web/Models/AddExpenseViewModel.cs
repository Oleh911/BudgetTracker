using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BudgetTracker.Web.Models;

public sealed class AddExpenseViewModel
{
    public Guid BudgetId { get; init; }
    public string BudgetName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Підкатегорія обов'язкова")]
    public Guid SubcategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Сума повинна бути більше нуля")]
    public decimal Amount { get; set; }

    [Required]
    public DateTimeOffset OccurredAt { get; set; }

    [MaxLength(500)]
    public string? Note { get; set; }

    public List<SelectListItem> Subcategories { get; set; } = [];
}