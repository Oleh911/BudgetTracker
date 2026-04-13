using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public CategoryKind Kind { get; private set; }
    public string? Color { get; private set; }
    public string? Icon { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Category() { }

    public Category(string name, CategoryKind kind)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        Id = Guid.NewGuid();
        Name = name.Trim();
        Kind = kind;
        DisplayOrder = 0;
        IsArchived = false;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}