using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public sealed class BudgetOperation
{
    public Guid Id { get; private set; }
    public OperationKind Kind { get; private set; }

    public Guid? BudgetId { get; private set; }
    public Guid? SourceBudgetId { get; private set; }
    public Guid? TargetBudgetId { get; private set; }

    public decimal? Amount { get; private set; }
    public decimal? DebitAmount { get; private set; }
    public decimal? CreditAmount { get; private set; }

    public Guid? SubcategoryId { get; private set; }

    public string? Note { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private BudgetOperation()
    {
    }

    public static BudgetOperation CreateExpense(
        Guid budgetId,
        Guid subcategoryId,
        decimal amount,
        DateTimeOffset occurredAt,
        string? note = null)
    {
        if (budgetId == Guid.Empty) throw new ArgumentException("BudgetId required.", nameof(budgetId));
        if (subcategoryId == Guid.Empty) throw new ArgumentException("SubcategoryId required.", nameof(subcategoryId));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        return new BudgetOperation
        {
            Id = Guid.NewGuid(),
            Kind = OperationKind.Expense,
            BudgetId = budgetId,
            SubcategoryId = subcategoryId,
            Amount = amount,
            OccurredAt = occurredAt,
            Note = Normalize(note),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static BudgetOperation CreateIncome(
        Guid budgetId,
        Guid subcategoryId,
        decimal amount,
        DateTimeOffset occurredAt,
        string? note = null)
    {
        if (budgetId == Guid.Empty) throw new ArgumentException("BudgetId required.", nameof(budgetId));
        if (subcategoryId == Guid.Empty) throw new ArgumentException("SubcategoryId required.", nameof(subcategoryId));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        return new BudgetOperation
        {
            Id = Guid.NewGuid(),
            Kind = OperationKind.Income,
            BudgetId = budgetId,
            SubcategoryId = subcategoryId,
            Amount = amount,
            OccurredAt = occurredAt,
            Note = Normalize(note),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static BudgetOperation CreateTransfer(
        Guid sourceBudgetId,
        Guid targetBudgetId,
        decimal debitAmount,
        decimal creditAmount,
        DateTimeOffset occurredAt,
        string? note = null)
    {
        if (sourceBudgetId == Guid.Empty) throw new ArgumentException("SourceBudgetId required.", nameof(sourceBudgetId));
        if (targetBudgetId == Guid.Empty) throw new ArgumentException("TargetBudgetId required.", nameof(targetBudgetId));
        if (sourceBudgetId == targetBudgetId) throw new ArgumentException("Source and target budgets must be different.");
        if (debitAmount <= 0) throw new ArgumentOutOfRangeException(nameof(debitAmount), "DebitAmount must be greater than zero.");
        if (creditAmount <= 0) throw new ArgumentOutOfRangeException(nameof(creditAmount), "CreditAmount must be greater than zero.");

        return new BudgetOperation
        {
            Id = Guid.NewGuid(),
            Kind = OperationKind.Transfer,
            SourceBudgetId = sourceBudgetId,
            TargetBudgetId = targetBudgetId,
            DebitAmount = debitAmount,
            CreditAmount = creditAmount,
            OccurredAt = occurredAt,
            Note = Normalize(note),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public void UpdateNote(string? note)
    {
        Note = Normalize(note);
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void UpdateOccurredAt(DateTimeOffset occurredAt)
    {
        OccurredAt = occurredAt;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static string? Normalize(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}