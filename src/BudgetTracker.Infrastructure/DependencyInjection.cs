using BudgetTracker.Application.Common.Abstractions;
using BudgetTracker.Domain.Enums;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BudgetTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BudgetTrackerDatabase")
            ?? throw new InvalidOperationException("Connection string 'BudgetTrackerDatabase' was not found.");

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.MapEnum<CurrencyCode>("currency_code");
        dataSourceBuilder.MapEnum<CategoryKind>("category_kind");
        dataSourceBuilder.MapEnum<OperationKind>("operation_kind");

        var dataSource = dataSourceBuilder.Build();

        services.AddSingleton(dataSource);

        services.AddDbContext<ApplicationDbContext>((provider, options) =>
            options.UseNpgsql(provider.GetRequiredService<NpgsqlDataSource>(), npgsql =>
            {
                npgsql.EnableRetryOnFailure();
            })
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
