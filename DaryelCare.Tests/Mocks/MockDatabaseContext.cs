using DaryelCare.Data;
using Npgsql;

namespace DaryelCare.Tests.Mocks;

public class MockDatabaseContext : DatabaseContext
{
    public MockDatabaseContext() : base(CreateMockConfiguration())
    {
    }

    public override NpgsqlConnection CreateConnection()
    {
        throw new NotImplementedException("Mock database - connections not supported");
    }

    public override async Task InitSchema()
    {
        await Task.CompletedTask;
        // No-op for tests
    }

    public override async Task Seed()
    {
        await Task.CompletedTask;
        // No-op for tests
    }

    private static IConfiguration CreateMockConfiguration()
    {
        var inMemorySettings = new Dictionary<string, string>
        {
            {"ConnectionStrings:DefaultConnection", "Host=localhost;Port=5432;Database=test"}
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }
}
