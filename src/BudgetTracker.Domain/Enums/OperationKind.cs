using NpgsqlTypes;

namespace BudgetTracker.Domain.Enums;

public enum OperationKind
{
    [PgName("expense")]
    Expense,
    [PgName("income")]
    Income,
    [PgName("transfer")]
    Transfer
}