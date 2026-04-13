namespace BudgetTracker.Domain.Entities;

public sealed class Subcategory
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Subcategory() { }

    public Subcategory(Guid categoryId, string name)
    {
        if (categoryId == Guid.Empty) throw new ArgumentException("categoryId required", nameof(categoryId));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));

        Id = Guid.NewGuid();
        CategoryId = categoryId;
        Name = name.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}