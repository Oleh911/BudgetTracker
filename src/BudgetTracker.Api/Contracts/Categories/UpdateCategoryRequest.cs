using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Api.Contracts.Categories;

public sealed class UpdateCategoryRequest
{
    public string Name { get; init; } = string.Empty;
    public CategoryKind Kind { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }
}