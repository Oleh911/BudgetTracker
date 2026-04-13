using BudgetTracker.Domain.Enums;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public sealed class Budget
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal AllocatedAmount { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public string? Note { get; private set; }
    public DateTimeOffset PeriodStart { get; private set; }
    public DateTimeOffset PeriodEnd { get; private set; }
    public bool IsArchived { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Budget() { }

    public Budget(string name, decimal allocatedAmount, CurrencyCode currency, DateTimeOffset periodStart, DateTimeOffset periodEnd, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        if (periodEnd <= periodStart) throw new ArgumentException("periodEnd must be after periodStart");

        Id = Guid.NewGuid();
        Name = name.Trim();
        AllocatedAmount = allocatedAmount >= 0 ? allocatedAmount : throw new ArgumentOutOfRangeException(nameof(allocatedAmount));
        Currency = currency;
        Note = note?.Trim();
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        IsArchived = false;
        DisplayOrder = 0;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}