using BudgetTracker.Domain.Entities;
using BudgetTracker.Web.Models;
using BudgetTracker.Web.Models.DTOs;

namespace BudgetTracker.Web.Services;

/// <summary>
/// Service for managing budgets
/// </summary>
public interface IBudgetService
{
    /// <summary>
    /// Gets all active budgets
    /// </summary>
    Task<IReadOnlyList<Budget>> GetAllBudgetsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets budget details by ID
    /// </summary>
    Task<BudgetDetailsDto?> GetBudgetDetailsAsync(Guid budgetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new budget
    /// </summary>
    Task<Guid> CreateBudgetAsync(CreateBudgetViewModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Archives a budget
    /// </summary>
    Task<bool> ArchiveBudgetAsync(Guid budgetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds an expense to a budget
    /// </summary>
    Task<Guid> AddExpenseAsync(AddExpenseViewModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets budget statistics
    /// </summary>
    Task<(int budgetsCount, int operationsCount)> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
