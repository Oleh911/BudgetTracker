namespace BudgetTracker.Domain.Entities;

public sealed class Subcategory
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string Name { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Subcategory()
    {
    }

    public Subcategory(Guid categoryId, string name)
    {
        Validate(categoryId, name);

        Id = Guid.NewGuid();
        CategoryId = categoryId;
        Name = name.Trim();
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Update(Guid categoryId, string name)
    {
        Validate(categoryId, name);

        CategoryId = categoryId;
        Name = name.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    private static void Validate(Guid categoryId, string name)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("CategoryId required.", nameof(categoryId));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name required.", nameof(name));
        }
    }
}