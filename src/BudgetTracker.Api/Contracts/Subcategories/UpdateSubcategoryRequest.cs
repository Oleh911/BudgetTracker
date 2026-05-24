namespace BudgetTracker.Api.Contracts.Subcategories;

public sealed class UpdateSubcategoryRequest
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
}   