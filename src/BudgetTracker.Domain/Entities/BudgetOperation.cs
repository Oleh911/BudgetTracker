using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public sealed class BudgetOperation
{
    public Guid Id { get; private set; }
    public OperationKind Kind { get; private set; }

    // For expense/income:
    public Guid? BudgetId { get; private set; }

    // For transfer:
    public Guid? SourceBudgetId { get; private set; }
    public Guid? TargetBudgetId { get; private set; }

    // Expense / Income amount
    public decimal? Amount { get; private set; }

    // Transfer amounts (supporting multi-currency)
    public decimal? DebitAmount { get; private set; }
    public decimal? CreditAmount { get; private set; }

    // Categorization (only subcategory stored)
    public Guid? SubcategoryId { get; private set; }

    public string? Note { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private BudgetOperation() { }

    // Factory methods would be used from Application layer.
    public static BudgetOperation CreateExpense(Guid budgetId, Guid subcategoryId, decimal amount, DateTimeOffset occurredAt, string? note = null)
    {
        if (budgetId == Guid.Empty) throw new ArgumentException("budgetId required");
        if (subcategoryId == Guid.Empty) throw new ArgumentException("subcategoryId required");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        return new BudgetOperation
        {
            Id = Guid.NewGuid(),
            Kind = OperationKind.Expense,
            BudgetId = budgetId,
            Amount = amount,
            SubcategoryId = subcategoryId,
            OccurredAt = occurredAt,
            Note = note?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static BudgetOperation CreateIncome(Guid budgetId, Guid subcategoryId, decimal amount, DateTimeOffset occurredAt, string? note = null)
    {
        if (budgetId == Guid.Empty) throw new ArgumentException("budgetId required");
        if (subcategoryId == Guid.Empty) throw new ArgumentException("subcategoryId required");
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));

        return new BudgetOperation
        {
            Id = Guid.NewGuid(),
            Kind = OperationKind.Income,
            BudgetId = budgetId,
            Amount = amount,
            SubcategoryId = subcategoryId,
            OccurredAt = occurredAt,
            Note = note?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }

    public static BudgetOperation CreateTransfer(Guid sourceBudgetId, Guid targetBudgetId, decimal debitAmount, decimal creditAmount, DateTimeOffset occurredAt, string? note = null)
    {
        if (sourceBudgetId == Guid.Empty) throw new ArgumentException("sourceBudgetId required");
        if (targetBudgetId == Guid.Empty) throw new ArgumentException("targetBudgetId required");
        if (sourceBudgetId == targetBudgetId) throw new ArgumentException("source and target must be different");
        if (debitAmount <= 0 || creditAmount <= 0) throw new ArgumentOutOfRangeException("amounts must be > 0");

        return new BudgetOperation
        {
            Id = Guid.NewGuid(),
            Kind = OperationKind.Transfer,
            SourceBudgetId = sourceBudgetId,
            TargetBudgetId = targetBudgetId,
            DebitAmount = debitAmount,
            CreditAmount = creditAmount,
            OccurredAt = occurredAt,
            Note = note?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}