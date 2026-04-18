namespace BudgetTracker.Api.Contracts.Subcategories;

public sealed class CreateSubcategoryRequest
{
    public Guid CategoryId { get; init; }
    public string Name { get; init; } = string.Empty;
}