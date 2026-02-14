using Npgsql;

namespace DaryelCare.Data;

/// <summary>
/// Manages the Npgsql data source for PostgreSQL connections.
/// </summary>
public class DatabaseContext
{
    private readonly NpgsqlDataSource _dataSource;

    public DatabaseContext(IConfiguration config)
    {
        var connStr = Environment.GetEnvironmentVariable("DATABASE_URL") is { } url
            ? ConvertPostgresUrl(url)
            : config.GetConnectionString("DefaultConnection")
              ?? "Host=localhost;Port=5432;Database=readykids";

        _dataSource = NpgsqlDataSource.Create(connStr);
    }

    public NpgsqlConnection CreateConnection() => _dataSource.CreateConnection();

    public async Task InitSchema()
    {
        var schemaPath = FindFile("db", "schema.sql");
        if (schemaPath is null)
            return;

        var sql = await File.ReadAllTextAsync(schemaPath);
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task Seed()
    {
        await InitSchema();
        var seedPath = FindFile("db", "seed.sql");
        if (seedPath is null)
            return;

        var sql = await File.ReadAllTextAsync(seedPath);
        await using var conn = CreateConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }

    // Search candidate paths for SQL files across local dev and container layouts.
    private static string? FindFile(string folder, string fileName)
    {
        string[] candidates =
        [
            Path.Combine(AppContext.BaseDirectory, folder, fileName),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", folder, fileName),
            Path.Combine(Directory.GetCurrentDirectory(), "..", folder, fileName),
            Path.Combine(Directory.GetCurrentDirectory(), folder, fileName),
        ];
        return candidates.FirstOrDefault(File.Exists);
    }

    private static string ConvertPostgresUrl(string url)
    {
        var uri = new Uri(url.Replace("postgres://", "postgresql://"));
        var userInfo = uri.UserInfo.Split(':');
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var db = uri.AbsolutePath.TrimStart('/');
        var user = userInfo.Length > 0 ? userInfo[0] : "postgres";
        var pass = userInfo.Length > 1 ? userInfo[1] : "";
        return $"Host={host};Port={port};Database={db};Username={user};Password={pass}";
    }
}
