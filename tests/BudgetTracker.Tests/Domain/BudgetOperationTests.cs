using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;

namespace BudgetTracker.Tests.Domain;

public sealed class BudgetOperationTests
{
    [Fact]
    public void CreateExpense_Should_CreateOperation_When_InputIsValid()
    {
        var budgetId = Guid.NewGuid();
        var subcategoryId = Guid.NewGuid();
        var occurredAt = DateTimeOffset.UtcNow;

        var op = BudgetOperation.CreateExpense(budgetId, subcategoryId, 100m, occurredAt, "note");

        Assert.Equal(OperationKind.Expense, op.Kind);
        Assert.Equal(budgetId, op.BudgetId);
        Assert.Equal(subcategoryId, op.SubcategoryId);
        Assert.Equal(100m, op.Amount);
        Assert.Equal("note", op.Note);
    }

    [Fact]
    public void CreateExpense_Should_Throw_When_AmountIsZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BudgetOperation.CreateExpense(Guid.NewGuid(), Guid.NewGuid(), 0m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateExpense_Should_Throw_When_BudgetIdIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            BudgetOperation.CreateExpense(Guid.Empty, Guid.NewGuid(), 100m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void CreateIncome_Should_CreateOperation_When_InputIsValid()
    {
        var op = BudgetOperation.CreateIncome(Guid.NewGuid(), Guid.NewGuid(), 500m, DateTimeOffset.UtcNow);

        Assert.Equal(OperationKind.Income, op.Kind);
        Assert.Equal(500m, op.Amount);
    }

    [Fact]
    public void CreateTransfer_Should_CreateOperation_When_InputIsValid()
    {
        var source = Guid.NewGuid();
        var target = Guid.NewGuid();

        var op = BudgetOperation.CreateTransfer(source, target, 1000m, 950m, DateTimeOffset.UtcNow);

        Assert.Equal(OperationKind.Transfer, op.Kind);
        Assert.Equal(source, op.SourceBudgetId);
        Assert.Equal(target, op.TargetBudgetId);
        Assert.Equal(1000m, op.DebitAmount);
        Assert.Equal(950m, op.CreditAmount);
    }

    [Fact]
    public void CreateTransfer_Should_Throw_When_SourceAndTargetAreSame()
    {
        var id = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            BudgetOperation.CreateTransfer(id, id, 100m, 100m, DateTimeOffset.UtcNow));
    }

    [Fact]
    public void UpdateNote_Should_ChangeNote()
    {
        var op = BudgetOperation.CreateExpense(Guid.NewGuid(), Guid.NewGuid(), 100m, DateTimeOffset.UtcNow, "old");

        op.UpdateNote("new note");

        Assert.Equal("new note", op.Note);
    }

    [Fact]
    public void UpdateOccurredAt_Should_ChangeDate()
    {
        var op = BudgetOperation.CreateExpense(Guid.NewGuid(), Guid.NewGuid(), 100m, DateTimeOffset.UtcNow);
        var newDate = DateTimeOffset.UtcNow.AddDays(-1);

        op.UpdateOccurredAt(newDate);

        Assert.Equal(newDate, op.OccurredAt);
    }
}