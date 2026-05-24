using Microsoft.EntityFrameworkCore;

namespace BudgetTracker.Tests.Api;

public abstract class ControllerTestsBase
{
    protected static TestApplicationDbContext CreateDbContext(string databaseNamePrefix)
    {
        var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
            .UseInMemoryDatabase($"{databaseNamePrefix}-{Guid.NewGuid():N}")
            .Options;

        return new TestApplicationDbContext(options);
    }
}