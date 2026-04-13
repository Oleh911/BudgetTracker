using BudgetTracker.Infrastructure;
using BudgetTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health/db", async (ApplicationDbContext dbContext, CancellationToken cancellationToken) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

    if (!canConnect)
    {
        return Results.Problem(
            title: "Database connection failed",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    var budgetsCount = await dbContext.Budgets.CountAsync(cancellationToken);

    return Results.Ok(new
    {
        status = "ok",
        database = "connected",
        budgetsCount
    });
});

app.Run();

public partial class Program;