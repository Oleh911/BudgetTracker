using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Api.Contracts.Budgets;

public sealed class CreateBudgetRequest
{
    public string Name { get; init; } = string.Empty;
    public decimal AllocatedAmount { get; init; }
    public CurrencyCode Currency { get; init; }
    public string? Note { get; init; }
}