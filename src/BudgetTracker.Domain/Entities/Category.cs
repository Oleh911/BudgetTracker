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

    private Category()
    {
    }

    public Category(string name, CategoryKind kind, string? color = null, string? icon = null)
    {
        Validate(name, kind);

        Id = Guid.NewGuid();
        Name = name.Trim();
        Kind = kind;
        Color = NormalizeOptional(color);
        Icon = NormalizeOptional(icon);
        DisplayOrder = 0;
        IsArchived = false;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Update(string name, CategoryKind kind, string? color, string? icon)
    {
        Validate(name, kind);

        Name = name.Trim();
        Kind = kind;
        Color = NormalizeOptional(color);
        Icon = NormalizeOptional(icon);
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

    private static void Validate(string name, CategoryKind kind)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name required.", nameof(name));
        }

        if (!Enum.IsDefined(kind))
        {
            throw new ArgumentOutOfRangeException(nameof(kind), "Invalid category kind.");
        }
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}