using BudgetTracker.Api.Contracts.Subcategories;
using BudgetTracker.Api.Controllers;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Api;

public sealed class SubcategoriesControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task GetAll_Should_FilterByCategoryId_When_CategoryIdProvided()
    {
        await using var db = CreateDbContext("subcategory-tests");
        var food = new Category("Food", CategoryKind.Expense);
        var transport = new Category("Transport", CategoryKind.Expense);

        db.Categories.AddRange(food, transport);
        db.Subcategories.AddRange(
            new Subcategory(food.Id, "Groceries"),
            new Subcategory(transport.Id, "Taxi"));

        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        var result = await controller.GetAll(food.Id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var items = Assert.IsAssignableFrom<IReadOnlyCollection<SubcategoryResponse>>(ok.Value);
        var item = Assert.Single(items);

        Assert.Equal("Groceries", item.Name);
        Assert.Equal(food.Id, item.CategoryId);
    }

    [Fact]
    public async Task GetById_Should_ReturnNotFound_When_SubcategoryDoesNotExist()
    {
        await using var db = CreateDbContext("subcategory-tests");
        var controller = new SubcategoriesController(db);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_Should_ReturnBadRequest_When_CategoryDoesNotExist()
    {
        await using var db = CreateDbContext("subcategory-tests");
        var controller = new SubcategoriesController(db);

        var request = new CreateSubcategoryRequest
        {
            CategoryId = Guid.NewGuid(),
            Name = "Groceries"
        };

        var result = await controller.Create(request, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);
    }

    [Fact]
    public async Task Create_Should_PersistSubcategory_When_InputIsValid()
    {
        await using var db = CreateDbContext("subcategory-tests");
        var category = new Category("Food", CategoryKind.Expense);
        db.Categories.Add(category);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        var request = new CreateSubcategoryRequest
        {
            CategoryId = category.Id,
            Name = "Groceries"
        };

        var result = await controller.Create(request, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var response = Assert.IsType<SubcategoryResponse>(created.Value);

        Assert.Equal("Groceries", response.Name);
        Assert.Equal(category.Id, response.CategoryId);
        Assert.Equal(1, await db.Subcategories.CountAsync());
    }

    [Fact]
    public async Task Update_Should_ModifySubcategory_When_InputIsValid()
    {
        await using var db = CreateDbContext("subcategory-tests");
        var oldCategory = new Category("Food", CategoryKind.Expense);
        var newCategory = new Category("Transport", CategoryKind.Expense);

        db.Categories.AddRange(oldCategory, newCategory);
        var subcategory = new Subcategory(oldCategory.Id, "Groceries");
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        var request = new UpdateSubcategoryRequest
        {
            CategoryId = newCategory.Id,
            Name = "Taxi"
        };

        var result = await controller.Update(subcategory.Id, request, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<SubcategoryResponse>(ok.Value);

        Assert.Equal("Taxi", response.Name);
        Assert.Equal(newCategory.Id, response.CategoryId);
    }

    [Fact]
    public async Task Delete_Should_RemoveSubcategory()
    {
        await using var db = CreateDbContext("subcategory-tests");
        var category = new Category("Food", CategoryKind.Expense);
        db.Categories.Add(category);

        var subcategory = new Subcategory(category.Id, "Groceries");
        db.Subcategories.Add(subcategory);
        await db.SaveChangesAsync();

        var controller = new SubcategoriesController(db);

        var result = await controller.Delete(subcategory.Id, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await db.Subcategories.CountAsync());
    }
}