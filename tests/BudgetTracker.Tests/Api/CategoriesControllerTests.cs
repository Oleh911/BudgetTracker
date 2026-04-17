using BudgetTracker.Api.Contracts.Categories;
using BudgetTracker.Api.Controllers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Api;

public sealed class CategoriesControllerTests
{
    [Fact]
    public async Task GetAll_Should_ExcludeArchived_ByDefault()
    {
        var db = CreateDbContext();
        db.Categories.Add(new Category("Food", CategoryKind.Expense));
        var archived = new Category("Salary", CategoryKind.Income);
        archived.Archive();
        db.Categories.Add(archived);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        var result = await controller.GetAll(false, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IReadOnlyCollection<CategoryResponse>>(ok.Value);
        Assert.Single(items);
        Assert.Equal("Food", items.Single().Name);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_InvalidInput()
    {
        var db = CreateDbContext();
        var controller = new CategoriesController(db);

        var request = new CreateCategoryRequest
        {
            Name = "",
            Kind = (CategoryKind)999
        };

        var result = await controller.Create(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_Should_PersistCategory_When_ValidInput()
    {
        var db = CreateDbContext();
        var controller = new CategoriesController(db);

        var request = new CreateCategoryRequest
        {
            Name = "Transport",
            Kind = CategoryKind.Expense,
            Color = "#00FFAA",
            Icon = "car"
        };

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<CategoryResponse>(created.Value);
        Assert.Equal("Transport", response.Name);
        Assert.Equal(CategoryKind.Expense, response.Kind);

        Assert.Equal(1, await db.Categories.CountAsync());
    }

    [Fact]
    public async Task Delete_Should_ArchiveCategory()
    {
        var db = CreateDbContext();
        var category = new Category("Investments", CategoryKind.Income);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        var result = await controller.Delete(category.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        var saved = await db.Categories.FirstAsync(x => x.Id == category.Id);
        Assert.True(saved.IsArchived);
    }

    private static TestApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase($"category-tests-{Guid.NewGuid()}")
            .Options;

        return new TestApplicationDbContext(options);
    }
}