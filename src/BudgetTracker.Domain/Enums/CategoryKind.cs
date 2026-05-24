using NpgsqlTypes;

namespace BudgetTracker.Domain.Enums;

public enum CategoryKind
{
    [PgName("expense")]
    Expense,
    [PgName("income")]
    Income
}