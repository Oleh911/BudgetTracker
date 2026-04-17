using BudgetTracker.Api.Contracts.Categories;
using BudgetTracker.Api.Controllers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Api;

public sealed class CategoriesControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task GetAll_Should_ExcludeArchived_ByDefault()
    {
        await using var db = CreateDbContext("category-tests");
        db.Categories.Add(new Category("Food", CategoryKind.Expense));

        var archived = new Category("Salary", CategoryKind.Income);
        archived.Archive();
        db.Categories.Add(archived);

        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        var result = await controller.GetAll(false, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IReadOnlyCollection<CategoryResponse>>(ok.Value);
        var item = Assert.Single(items);

        Assert.Equal("Food", item.Name);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_CategoryDoesNotExist()
    {
        await using var db = CreateDbContext("category-tests");
        var controller = new CategoriesController(db);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_InvalidInput()
    {
        await using var db = CreateDbContext("category-tests");
        var controller = new CategoriesController(db);

        var request = new CreateCategoryRequest
        {
            Name = string.Empty,
            Kind = (CategoryKind)999
        };

        var result = await controller.Create(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_Should_PersistCategory_When_ValidInput()
    {
        await using var db = CreateDbContext("category-tests");
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
    public async Task Update_Should_ModifyCategory_When_InputIsValid()
    {
        await using var db = CreateDbContext("category-tests");
        var category = new Category("Food", CategoryKind.Expense);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        var request = new UpdateCategoryRequest
        {
            Name = "Salary",
            Kind = CategoryKind.Income,
            Color = "#FFFFFF",
            Icon = "wallet"
        };

        var result = await controller.Update(category.Id, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<CategoryResponse>(ok.Value);

        Assert.Equal("Salary", response.Name);
        Assert.Equal(CategoryKind.Income, response.Kind);
        Assert.Equal("#FFFFFF", response.Color);
        Assert.Equal("wallet", response.Icon);
    }

    [Fact]
    public async Task Delete_Should_ArchiveCategory()
    {
        await using var db = CreateDbContext("category-tests");
        var category = new Category("Investments", CategoryKind.Income);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        var result = await controller.Delete(category.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);

        var saved = await db.Categories.FirstAsync(x => x.Id == category.Id);
        Assert.True(saved.IsArchived);
    }
}