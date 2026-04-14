using BudgetTracker.Api.Contracts.Budgets;
using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace BudgetTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BudgetsController(IApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BudgetResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BudgetResponse>>> GetAll(
        [FromQuery] bool includeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Budgets.AsNoTracking().AsQueryable();

        if (!includeArchived)
        {
            query = query.Where(x => !x.IsArchived);
        }

        var budgets = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);

        return Ok(budgets.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var budget = await dbContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (budget is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(budget));
    }

    [HttpPost]
    [ProducesResponseType(typeof(BudgetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BudgetResponse>> Create(
        [FromBody] CreateBudgetRequest request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request.Name, request.AllocatedAmount);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var budget = new Budget(
            request.Name,
            request.AllocatedAmount,
            request.Currency,
            request.Note);

        dbContext.Budgets.Add(budget);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict("A budget with the same name already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = budget.Id }, MapToResponse(budget));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BudgetResponse>> Update(
        Guid id,
        [FromBody] UpdateBudgetRequest request,
        CancellationToken cancellationToken)
    {
        ValidateRequest(request.Name, request.AllocatedAmount);

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var budget = await dbContext.Budgets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (budget is null)
        {
            return NotFound();
        }

        budget.Update(
            request.Name,
            request.AllocatedAmount,
            request.Currency,
            request.Note);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            return Conflict("A budget with the same name already exists.");
        }

        return Ok(MapToResponse(budget));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var budget = await dbContext.Budgets.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (budget is null)
        {
            return NotFound();
        }

        budget.Archive();
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private void ValidateRequest(string name, decimal allocatedAmount)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ModelState.AddModelError(nameof(name), "Name is required.");
        }

        if (allocatedAmount < 0)
        {
            ModelState.AddModelError(nameof(allocatedAmount), "AllocatedAmount cannot be negative.");
        }
    }

    private static bool IsUniqueViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };
    }

    private static BudgetResponse MapToResponse(Budget budget)
    {
        return new BudgetResponse
        {
            Id = budget.Id,
            Name = budget.Name,
            AllocatedAmount = budget.AllocatedAmount,
            Currency = budget.Currency,
            Note = budget.Note,
            IsArchived = budget.IsArchived,
            DisplayOrder = budget.DisplayOrder,
            CreatedAt = budget.CreatedAt,
            UpdatedAt = budget.UpdatedAt
        };
    }
}