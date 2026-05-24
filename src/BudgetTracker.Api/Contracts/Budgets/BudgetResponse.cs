using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Api.Contracts.Budgets;

public sealed class BudgetResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal AllocatedAmount { get; init; }
    public CurrencyCode Currency { get; init; }
    public string? Note { get; init; }
    public bool IsArchived { get; init; }
    public int DisplayOrder { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}