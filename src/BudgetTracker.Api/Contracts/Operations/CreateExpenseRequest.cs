namespace BudgetTracker.Api.Contracts.Operations;

public sealed class CreateExpenseRequest
{
    public Guid BudgetId { get; init; }
    public Guid SubcategoryId { get; init; }
    public decimal Amount { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public string? Note { get; init; }
}