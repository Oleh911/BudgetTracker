using BudgetTracker.Api.Contracts.Budgets;
using BudgetTracker.Api.Controllers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Api;

public sealed class BudgetsControllerTests
{
    [Fact]
    public async Task GetAll_Should_ExcludeArchived_ByDefault()
    {
        var db = CreateDbContext();
        db.Budgets.Add(new Budget("Home", 1000m, CurrencyCode.UAH));
        var archived = new Budget("Travel", 500m, CurrencyCode.UAH);
        archived.Archive();
        db.Budgets.Add(archived);
        await db.SaveChangesAsync();

        var controller = new BudgetsController(db);

        var result = await controller.GetAll(false, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IReadOnlyCollection<BudgetResponse>>(ok.Value);
        Assert.Single(items);
        Assert.Equal("Home", items.Single().Name);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_InvalidInput()
    {
        var db = CreateDbContext();
        var controller = new BudgetsController(db);

        var request = new CreateBudgetRequest
        {
            Name = "",
            AllocatedAmount = -10m,
            Currency = CurrencyCode.USD
        };

        var result = await controller.Create(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_Should_PersistBudget_When_ValidInput()
    {
        var db = CreateDbContext();
        var controller = new BudgetsController(db);

        var request = new CreateBudgetRequest
        {
            Name = "Health",
            AllocatedAmount = 1200m,
            Currency = CurrencyCode.EUR,
            Note = "Monthly health budget"
        };

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<BudgetResponse>(created.Value);
        Assert.Equal("Health", response.Name);

        Assert.Equal(1, await db.Budgets.CountAsync());
    }

    [Fact]
    public async Task Delete_Should_ArchiveBudget()
    {
        var db = CreateDbContext();
        var budget = new Budget("Food", 2000m, CurrencyCode.UAH);
        db.Budgets.Add(budget);
        await db.SaveChangesAsync();

        var controller = new BudgetsController(db);

        var result = await controller.Delete(budget.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        var saved = await db.Budgets.FirstAsync(x => x.Id == budget.Id);
        Assert.True(saved.IsArchived);
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase($"budget-tests-{Guid.NewGuid()}")
            .Options;

        return new TestApplicationDbContext(options);
    }
}