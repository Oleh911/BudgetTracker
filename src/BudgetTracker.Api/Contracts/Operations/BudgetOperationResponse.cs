using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Api.Contracts.Operations;

public sealed class BudgetOperationResponse
{
    public Guid Id { get; init; }
    public OperationKind Kind { get; init; }

    public Guid? BudgetId { get; init; }
    public Guid? SourceBudgetId { get; init; }
    public Guid? TargetBudgetId { get; init; }

    public decimal? Amount { get; init; }
    public decimal? DebitAmount { get; init; }
    public decimal? CreditAmount { get; init; }

    public Guid? SubcategoryId { get; init; }

    public string? Note { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}