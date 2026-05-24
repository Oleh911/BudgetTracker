using BudgetTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Budget> Budgets { get; }
    DbSet<Category> Categories { get; }
    DbSet<Subcategory> Subcategories { get; }
    DbSet<BudgetOperation> BudgetOperations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}