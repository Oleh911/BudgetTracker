using BudgetTracker.Api.Contracts.Subcategories;
using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BudgetTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SubcategoriesController(IApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SubcategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<SubcategoryResponse>>> GetAll(
        [FromQuery] Guid? categoryId,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Subcategories
            .AsNoTracking()
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(x => x.CategoryId == categoryId.Value);
        }

        var subcategories = await query
            .OrderBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(subcategories.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SubcategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SubcategoryResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var subcategory = await dbContext.Subcategories
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (subcategory is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(subcategory));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SubcategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubcategoryResponse>> Create(
        [FromBody] CreateSubcategoryRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateRequestAsync(request.CategoryId, request.Name, cancellationToken);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var subcategory = new Subcategory(request.CategoryId, request.Name);

        dbContext.Subcategories.Add(subcategory);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict("A subcategory with the same name already exists in this category.");
        }

        return CreatedAtAction(nameof(GetById), new { id = subcategory.Id }, MapToResponse(subcategory));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SubcategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SubcategoryResponse>> Update(
        Guid id,
        [FromBody] UpdateSubcategoryRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateRequestAsync(request.CategoryId, request.Name, cancellationToken);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var subcategory = await dbContext.Subcategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (subcategory is null)
        {
            return NotFound();
        }

        subcategory.Update(request.CategoryId, request.Name);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict("A subcategory with the same name already exists in this category.");
        }

        return Ok(MapToResponse(subcategory));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var subcategory = await dbContext.Subcategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (subcategory is null)
        {
            return NotFound();
        }

        dbContext.Subcategories.Remove(subcategory);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsForeignKeyViolation(ex))
        {
            return Conflict("This subcategory is already used in operations and cannot be deleted.");
        }

        return NoContent();
    }

    private async Task ValidateRequestAsync(Guid categoryId, string name, CancellationToken cancellationToken)
    {
        if (categoryId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(CreateSubcategoryRequest.CategoryId), "CategoryId is required.");
        }
        else
        {
            var categoryExists = await dbContext.Categories
                .AsNoTracking()
                .AnyAsync(x => x.Id == categoryId, cancellationToken);

            if (!categoryExists)
            {
                ModelState.AddModelError(nameof(CreateSubcategoryRequest.CategoryId), "Category was not found.");
            }
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(nameof(CreateSubcategoryRequest.Name), "Name is required.");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }

    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation };
    }

    private static SubcategoryResponse MapToResponse(Subcategory subcategory)
    {
        return new SubcategoryResponse
        {
            Id = subcategory.Id,
            CategoryId = subcategory.CategoryId,
            Name = subcategory.Name,
            CreatedAt = subcategory.CreatedAt,
            UpdatedAt = subcategory.UpdatedAt
        };
    }
}