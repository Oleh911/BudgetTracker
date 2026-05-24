namespace BudgetTracker.Api.Contracts.Operations;

public sealed class CreateTransferRequest
{
    public Guid SourceBudgetId { get; init; }
    public Guid TargetBudgetId { get; init; }
    public decimal DebitAmount { get; init; }
    public decimal CreditAmount { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public string? Note { get; init; }
}