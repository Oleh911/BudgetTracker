using BudgetTracker.Api.Contracts.Operations;
using BudgetTracker.Api.Controllers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Api;

public sealed class BudgetOperationsControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task CreateExpense_Should_PersistOperation_When_InputIsValid()
    {
        await using var db = CreateDbContext("operations-tests");

        var budget = new Budget("Food", 1000m, CurrencyCode.UAH);
        var category = new Category("Food", CategoryKind.Expense);
        db.Budgets.Add(budget);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory(category.Id, "Groceries");
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var controller = new BudgetOperationsController(db);

        var request = new CreateExpenseRequest
        {
            BudgetId = budget.Id,
            SubcategoryId = subcategory.Id,
            Amount = 200m,
            OccurredAt = DateTimeOffset.UtcNow,
            Note = "Weekly groceries"
        };

        var result = await controller.CreateExpense(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<BudgetOperationResponse>(created.Value);

        Assert.Equal(OperationKind.Expense, response.Kind);
        Assert.Equal(200m, response.Amount);
        Assert.Equal(1, await db.BudgetOperations.CountAsync());
    }

    [Fact]
    public async Task CreateExpense_Should_ReturnBadRequest_When_BudgetNotFound()
    {
        await using var db = CreateDbContext("operations-tests");
        var controller = new BudgetOperationsController(db);

        var request = new CreateExpenseRequest
        {
            BudgetId = Guid.NewGuid(),
            SubcategoryId = Guid.NewGuid(),
            Amount = 100m,
            OccurredAt = DateTimeOffset.UtcNow
        };

        var result = await controller.CreateExpense(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task CreateTransfer_Should_PersistOperation_When_InputIsValid()
    {
        await using var db = CreateDbContext("operations-tests");

        var source = new Budget("Source", 1000m, CurrencyCode.UAH);
        var target = new Budget("Target", 2000m, CurrencyCode.EUR);
        db.Budgets.AddRange(source, target);
        await db.SaveChangesAsync();

        var controller = new BudgetOperationsController(db);

        var request = new CreateTransferRequest
        {
            SourceBudgetId = source.Id,
            TargetBudgetId = target.Id,
            DebitAmount = 1000m,
            CreditAmount = 25m,
            OccurredAt = DateTimeOffset.UtcNow
        };

        var result = await controller.CreateTransfer(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<BudgetOperationResponse>(created.Value);

        Assert.Equal(OperationKind.Transfer, response.Kind);
        Assert.Equal(1000m, response.DebitAmount);
        Assert.Equal(25m, response.CreditAmount);
    }

    [Fact]
    public async Task CreateTransfer_Should_ReturnBadRequest_When_SameBudgets()
    {
        await using var db = CreateDbContext("operations-tests");

        var budget = new Budget("Budget", 1000m, CurrencyCode.UAH);
        db.Budgets.Add(budget);
        await db.SaveChangesAsync();

        var controller = new BudgetOperationsController(db);

        var request = new CreateTransferRequest
        {
            SourceBudgetId = budget.Id,
            TargetBudgetId = budget.Id,
            DebitAmount = 500m,
            CreditAmount = 500m,
            OccurredAt = DateTimeOffset.UtcNow
        };

        var result = await controller.CreateTransfer(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task GetAll_Should_FilterByBudgetId()
    {
        await using var db = CreateDbContext("operations-tests");

        var budget1 = new Budget("Food", 1000m, CurrencyCode.UAH);
        var budget2 = new Budget("Travel", 2000m, CurrencyCode.USD);
        var category = new Category("Food", CategoryKind.Expense);
        db.Budgets.AddRange(budget1, budget2);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory(category.Id, "Groceries");
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        db.BudgetOperations.AddRange(
            BudgetOperation.CreateExpense(budget1.Id, subcategory.Id, 100m, DateTimeOffset.UtcNow),
            BudgetOperation.CreateExpense(budget2.Id, subcategory.Id, 200m, DateTimeOffset.UtcNow));

        await db.SaveChangesAsync();

        var controller = new BudgetOperationsController(db);

        var result = await controller.GetAll(budget1.Id, null, null, null, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IReadOnlyCollection<BudgetOperationResponse>>(ok.Value);

        Assert.Single(items);
        Assert.Equal(budget1.Id, items.Single().BudgetId);
    }

    [Fact]
    public async Task Delete_Should_RemoveOperation()
    {
        await using var db = CreateDbContext("operations-tests");

        var budget = new Budget("Food", 1000m, CurrencyCode.UAH);
        var category = new Category("Food", CategoryKind.Expense);
        db.Budgets.Add(budget);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var subcategory = new Subcategory(category.Id, "Groceries");
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var op = BudgetOperation.CreateExpense(budget.Id, subcategory.Id, 100m, DateTimeOffset.UtcNow);
        db.BudgetOperations.Add(op);
        await db.SaveChangesAsync();

        var controller = new BudgetOperationsController(db);

        var result = await controller.Delete(op.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await db.BudgetOperations.CountAsync());
    }
}