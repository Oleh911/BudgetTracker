using BudgetTracker.Api.Contracts.Operations;
using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class BudgetOperationsController(IApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<BudgetOperationResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<BudgetOperationResponse>>> GetAll(
        [FromQuery] Guid? budgetId,
        [FromQuery] OperationKind? kind,
        [FromQuery] DateTimeOffset? from,
        [FromQuery] DateTimeOffset? to,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.BudgetOperations.AsNoTracking().AsQueryable();

        if (budgetId.HasValue)
        {
            query = query.Where(x =>
                x.BudgetId == budgetId ||
                x.SourceBudgetId == budgetId ||
                x.TargetBudgetId == budgetId);
        }

        if (kind.HasValue)
        {
            query = query.Where(x => x.Kind == kind.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(x => x.OccurredAt >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.OccurredAt <= to.Value);
        }

        var operations = await query
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

        return Ok(operations.Select(MapToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetOperationResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var operation = await dbContext.BudgetOperations
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (operation is null)
        {
            return NotFound();
        }

        return Ok(MapToResponse(operation));
    }

    [HttpPost("expense")]
    [ProducesResponseType(typeof(BudgetOperationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetOperationResponse>> CreateExpense(
        [FromBody] CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateExpenseIncomeAsync(request.BudgetId, request.SubcategoryId, request.Amount, cancellationToken);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var operation = BudgetOperation.CreateExpense(
            request.BudgetId,
            request.SubcategoryId,
            request.Amount,
            request.OccurredAt,
            request.Note);

        dbContext.BudgetOperations.Add(operation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = operation.Id }, MapToResponse(operation));
    }

    [HttpPost("income")]
    [ProducesResponseType(typeof(BudgetOperationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetOperationResponse>> CreateIncome(
        [FromBody] CreateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateExpenseIncomeAsync(request.BudgetId, request.SubcategoryId, request.Amount, cancellationToken);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var operation = BudgetOperation.CreateIncome(
            request.BudgetId,
            request.SubcategoryId,
            request.Amount,
            request.OccurredAt,
            request.Note);

        dbContext.BudgetOperations.Add(operation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = operation.Id }, MapToResponse(operation));
    }

    [HttpPost("transfer")]
    [ProducesResponseType(typeof(BudgetOperationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(SerializableError), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BudgetOperationResponse>> CreateTransfer(
        [FromBody] CreateTransferRequest request,
        CancellationToken cancellationToken)
    {
        await ValidateTransferAsync(
            request.SourceBudgetId,
            request.TargetBudgetId,
            request.DebitAmount,
            request.CreditAmount,
            cancellationToken);

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var operation = BudgetOperation.CreateTransfer(
            request.SourceBudgetId,
            request.TargetBudgetId,
            request.DebitAmount,
            request.CreditAmount,
            request.OccurredAt,
            request.Note);

        dbContext.BudgetOperations.Add(operation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = operation.Id }, MapToResponse(operation));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(BudgetOperationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BudgetOperationResponse>> Update(
        Guid id,
        [FromBody] UpdateOperationRequest request,
        CancellationToken cancellationToken)
    {
        var operation = await dbContext.BudgetOperations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (operation is null)
        {
            return NotFound();
        }

        operation.UpdateNote(request.Note);
        operation.UpdateOccurredAt(request.OccurredAt);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Ok(MapToResponse(operation));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var operation = await dbContext.BudgetOperations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (operation is null)
        {
            return NotFound();
        }

        dbContext.BudgetOperations.Remove(operation);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private async Task ValidateExpenseIncomeAsync(
        Guid budgetId,
        Guid subcategoryId,
        decimal amount,
        CancellationToken cancellationToken)
    {
        if (budgetId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(CreateExpenseRequest.BudgetId), "BudgetId is required.");
        }
        else
        {
            var budgetExists = await dbContext.Budgets.AsNoTracking().AnyAsync(x => x.Id == budgetId, cancellationToken);
            if (!budgetExists)
            {
                ModelState.AddModelError(nameof(CreateExpenseRequest.BudgetId), "Budget was not found.");
            }
        }

        if (subcategoryId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(CreateExpenseRequest.SubcategoryId), "SubcategoryId is required.");
        }
        else
        {
            var subcategoryExists = await dbContext.Subcategories.AsNoTracking().AnyAsync(x => x.Id == subcategoryId, cancellationToken);
            if (!subcategoryExists)
            {
                ModelState.AddModelError(nameof(CreateExpenseRequest.SubcategoryId), "Subcategory was not found.");
            }
        }

        if (amount <= 0)
        {
            ModelState.AddModelError(nameof(CreateExpenseRequest.Amount), "Amount must be greater than zero.");
        }
    }

    private async Task ValidateTransferAsync(
        Guid sourceBudgetId,
        Guid targetBudgetId,
        decimal debitAmount,
        decimal creditAmount,
        CancellationToken cancellationToken)
    {
        if (sourceBudgetId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(CreateTransferRequest.SourceBudgetId), "SourceBudgetId is required.");
        }
        else
        {
            var exists = await dbContext.Budgets.AsNoTracking().AnyAsync(x => x.Id == sourceBudgetId, cancellationToken);
            if (!exists) ModelState.AddModelError(nameof(CreateTransferRequest.SourceBudgetId), "Source budget was not found.");
        }

        if (targetBudgetId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(CreateTransferRequest.TargetBudgetId), "TargetBudgetId is required.");
        }
        else
        {
            var exists = await dbContext.Budgets.AsNoTracking().AnyAsync(x => x.Id == targetBudgetId, cancellationToken);
            if (!exists) ModelState.AddModelError(nameof(CreateTransferRequest.TargetBudgetId), "Target budget was not found.");
        }

        if (sourceBudgetId != Guid.Empty && targetBudgetId != Guid.Empty && sourceBudgetId == targetBudgetId)
        {
            ModelState.AddModelError(nameof(CreateTransferRequest.TargetBudgetId), "Source and target budgets must be different.");
        }

        if (debitAmount <= 0)
        {
            ModelState.AddModelError(nameof(CreateTransferRequest.DebitAmount), "DebitAmount must be greater than zero.");
        }

        if (creditAmount <= 0)
        {
            ModelState.AddModelError(nameof(CreateTransferRequest.CreditAmount), "CreditAmount must be greater than zero.");
        }
    }

    private static BudgetOperationResponse MapToResponse(BudgetOperation operation)
    {
        return new BudgetOperationResponse
        {
            Id = operation.Id,
            Kind = operation.Kind,
            BudgetId = operation.BudgetId,
            SourceBudgetId = operation.SourceBudgetId,
            TargetBudgetId = operation.TargetBudgetId,
            Amount = operation.Amount,
            DebitAmount = operation.DebitAmount,
            CreditAmount = operation.CreditAmount,
            SubcategoryId = operation.SubcategoryId,
            Note = operation.Note,
            OccurredAt = operation.OccurredAt,
            CreatedAt = operation.CreatedAt,
            UpdatedAt = operation.UpdatedAt
        };
    }
}