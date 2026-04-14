using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Tests.Domain;

public sealed class BudgetTests
{
    [Fact]
    public void Constructor_Should_CreateBudget_When_InputIsValid()
    {
        var budget = new Budget("Home", 5000m, CurrencyCode.UAH, "Monthly");

        Assert.Equal("Home", budget.Name);
        Assert.Equal(5000m, budget.AllocatedAmount);
        Assert.Equal(CurrencyCode.UAH, budget.Currency);
        Assert.False(budget.IsArchived);
    }

    [Fact]
    public void Constructor_Should_Throw_When_NameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Budget("", 100m, CurrencyCode.USD));
    }

    [Fact]
    public void Constructor_Should_Throw_When_AmountIsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Budget("Food", -1m, CurrencyCode.USD));
    }

    [Fact]
    public void Update_Should_ChangeValues()
    {
        var budget = new Budget("Old", 100m, CurrencyCode.USD);

        budget.Update("New", 200m, CurrencyCode.EUR, "Updated");

        Assert.Equal("New", budget.Name);
        Assert.Equal(200m, budget.AllocatedAmount);
        Assert.Equal(CurrencyCode.EUR, budget.Currency);
        Assert.Equal("Updated", budget.Note);
    }

    [Fact]
    public void Archive_Should_SetIsArchivedTrue()
    {
        var budget = new Budget("Home", 100m, CurrencyCode.USD);

        budget.Archive();

        Assert.True(budget.IsArchived);
    }
}   