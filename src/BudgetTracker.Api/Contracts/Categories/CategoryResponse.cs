using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Api.Contracts.Categories;

public sealed class CategoryResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public CategoryKind Kind { get; init; }
    public string? Color { get; init; }
    public string? Icon { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsArchived { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}