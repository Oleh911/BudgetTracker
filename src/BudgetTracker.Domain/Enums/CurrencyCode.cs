using NpgsqlTypes;

namespace BudgetTracker.Domain.Enums;

public enum CurrencyCode
{
    [PgName("UAH")]
    UAH,
    [PgName("EUR")]
    EUR,
    [PgName("USD")]
    USD
}