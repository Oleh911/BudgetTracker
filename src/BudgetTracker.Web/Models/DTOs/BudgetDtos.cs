using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Web.Models.DTOs;

/// <summary>
/// Data transfer object for budget information
/// </summary>
public sealed record BudgetDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal AllocatedAmount { get; init; }
    public decimal SpentAmount { get; init; }
    public decimal RemainingAmount { get; init; }
    public CurrencyCode Currency { get; init; }
    public string? Note { get; init; }
    public bool IsArchived { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Data transfer object for budget operation
/// </summary>
public sealed record BudgetOperationDto
{
    public Guid Id { get; init; }
    public Guid BudgetId { get; init; }
    public Guid? SubcategoryId { get; init; }
    public string? SubcategoryName { get; init; }
    public OperationKind Kind { get; init; }
    public decimal? Amount { get; init; }
    public DateTimeOffset OccurredAt { get; init; }
    public string? Note { get; init; }
}

/// <summary>
/// Data transfer object for budget details with operations
/// </summary>
public sealed record BudgetDetailsDto
{
    public BudgetDto Budget { get; init; } = null!;
    public IReadOnlyList<BudgetOperationDto> Operations { get; init; } = Array.Empty<BudgetOperationDto>();
    public decimal TotalExpenses { get; init; }
    public decimal TotalIncome { get; init; }
    public decimal NetAmount { get; init; }
}
