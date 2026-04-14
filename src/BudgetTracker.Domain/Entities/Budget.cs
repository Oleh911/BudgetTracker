using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public sealed class Budget
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public decimal AllocatedAmount { get; private set; }
    public CurrencyCode Currency { get; private set; }
    public string? Note { get; private set; }
    public bool IsArchived { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Budget()
    {
    }

    public Budget(
        string name,
        decimal allocatedAmount,
        CurrencyCode currency,
        string? note = null)
    {
        Validate(name, allocatedAmount);

        Id = Guid.NewGuid();
        Name = name.Trim();
        AllocatedAmount = allocatedAmount;
        Currency = currency;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        IsArchived = false;
        DisplayOrder = 0;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Update(
        string name,
        decimal allocatedAmount,
        CurrencyCode currency,
        string? note)
    {
        Validate(name, allocatedAmount);

        Name = name.Trim();
        AllocatedAmount = allocatedAmount;
        Currency = currency;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Archive()
    {
        if (IsArchived)
        {
            return;
        }

        IsArchived = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void Validate(string name, decimal allocatedAmount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name required.", nameof(name));
        }

        if (allocatedAmount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(allocatedAmount), "Allocated amount cannot be negative.");
        }
    }
}