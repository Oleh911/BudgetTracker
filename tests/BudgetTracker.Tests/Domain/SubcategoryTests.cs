using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Tests.Domain;

public sealed class SubcategoryTests
{
    [Fact]
    public void Constructor_Should_CreateSubcategory_When_InputIsValid()
    {
        var categoryId = Guid.NewGuid();

        var subcategory = new Subcategory(categoryId, "Groceries");

        Assert.Equal(categoryId, subcategory.CategoryId);
        Assert.Equal("Groceries", subcategory.Name);
    }

    [Fact]
    public void Constructor_Should_Throw_When_CategoryIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Subcategory(Guid.Empty, "Groceries"));
    }

    [Fact]
    public void Constructor_Should_Throw_When_NameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Subcategory(Guid.NewGuid(), string.Empty));
    }

    [Fact]
    public void Update_Should_ModifyValues()
    {
        var subcategory = new Subcategory(Guid.NewGuid(), "Old");
        var newCategoryId = Guid.NewGuid();

        subcategory.Update(newCategoryId, "New");

        Assert.Equal(newCategoryId, subcategory.CategoryId);
        Assert.Equal("New", subcategory.Name);
    }
}