using BudgetTracker.Domain.Entities;

namespace BudgetTracker.Web.Models;

public sealed class BudgetDetailsViewModel
{
    public Budget Budget { get; init; } = null!;
    public IReadOnlyCollection<BudgetOperation> Operations { get; init; } = [];
    public decimal TotalExpenses { get; init; }
    public decimal TotalIncome { get; init; }
    public decimal Remaining => Budget.AllocatedAmount - TotalExpenses + TotalIncome;
}