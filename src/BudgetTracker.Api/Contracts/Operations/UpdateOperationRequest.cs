namespace BudgetTracker.Api.Contracts.Operations;

public sealed class UpdateOperationRequest
{
    public DateTimeOffset OccurredAt { get; init; }
    public string? Note { get; init; }
}