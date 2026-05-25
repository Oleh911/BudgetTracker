using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Entities;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Web.Models;
using BudgetTracker.Web.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Web.Services;

/// <summary>
/// Implementation of budget service
/// </summary>
public sealed class BudgetService : IBudgetService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ILogger<BudgetService> _logger;

    public BudgetService(IApplicationDbContext dbContext, ILogger<BudgetService> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IReadOnlyList<Budget>> GetAllBudgetsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching all active budgets");

        return await _dbContext.Budgets
            .AsNoTracking()
            .Where(x => !x.IsArchived)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<BudgetDetailsDto?> GetBudgetDetailsAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching budget details for {BudgetId}", budgetId);

        var budget = await _dbContext.Budgets
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == budgetId, cancellationToken);

        if (budget is null)
        {
            _logger.LogWarning("Budget {BudgetId} not found", budgetId);
            return null;
        }

        var operations = await _dbContext.BudgetOperations
            .AsNoTracking()
            .Where(x => x.BudgetId == budgetId)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(cancellationToken);

        var totalExpenses = operations
            .Where(x => x.Kind == OperationKind.Expense)
            .Sum(x => x.Amount ?? 0);

        var totalIncome = operations
            .Where(x => x.Kind == OperationKind.Income)
            .Sum(x => x.Amount ?? 0);

        return new BudgetDetailsDto
        {
            Budget = new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                AllocatedAmount = budget.AllocatedAmount,
                SpentAmount = totalExpenses,
                RemainingAmount = budget.AllocatedAmount - totalExpenses + totalIncome,
                Currency = budget.Currency,
                Note = budget.Note,
                IsArchived = budget.IsArchived,
                CreatedAt = budget.CreatedAt
            },
            Operations = operations.Select(o => new BudgetOperationDto
            {
                Id = o.Id,
                BudgetId = o.BudgetId ?? Guid.Empty,
                SubcategoryId = o.SubcategoryId,
                Kind = o.Kind,
                Amount = o.Amount,
                OccurredAt = o.OccurredAt,
                Note = o.Note
            }).ToList(),
            TotalExpenses = totalExpenses,
            TotalIncome = totalIncome,
            NetAmount = totalIncome - totalExpenses
        };
    }

    public async Task<Guid> CreateBudgetAsync(CreateBudgetViewModel model, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new budget: {BudgetName}", model.Name);

        var budget = new Budget(model.Name, model.AllocatedAmount, model.Currency, model.Note);

        _dbContext.Budgets.Add(budget);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Budget created with ID: {BudgetId}", budget.Id);

        return budget.Id;
    }

    public async Task<bool> ArchiveBudgetAsync(Guid budgetId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Archiving budget: {BudgetId}", budgetId);

        var budget = await _dbContext.Budgets
            .FirstOrDefaultAsync(x => x.Id == budgetId, cancellationToken);

        if (budget is null)
        {
            _logger.LogWarning("Budget {BudgetId} not found for archiving", budgetId);
            return false;
        }

        budget.Archive();
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Budget {BudgetId} archived successfully", budgetId);

        return true;
    }

    public async Task<Guid> AddExpenseAsync(AddExpenseViewModel model, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding expense to budget: {BudgetId}, Amount: {Amount}", model.BudgetId, model.Amount);

        var operation = BudgetOperation.CreateExpense(
            model.BudgetId,
            model.SubcategoryId,
            model.Amount,
            model.OccurredAt,
            model.Note);

        _dbContext.BudgetOperations.Add(operation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Expense added with ID: {OperationId}", operation.Id);

        return operation.Id;
    }

    public async Task<(int budgetsCount, int operationsCount)> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching statistics");

        var budgetsCount = await _dbContext.Budgets.CountAsync(cancellationToken);
        var operationsCount = await _dbContext.BudgetOperations.CountAsync(cancellationToken);

        return (budgetsCount, operationsCount);
    }
}
