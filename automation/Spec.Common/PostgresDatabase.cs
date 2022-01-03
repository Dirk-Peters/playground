using Dapper;
using Docker;
using Npgsql;

namespace Spec.Common;

public sealed class PostgresDatabase : IAsyncDisposable, IDisposable
{
    private readonly DockerContainer container;
    private readonly PostgresConnectionString connectionString;

    public PostgresDatabase(DockerContainer container, PostgresConnectionString connectionString)
    {
        this.container = container;
        this.connectionString = connectionString;
    }

    public string ConnectionString => connectionString.ToString();

    public NpgsqlConnection OpenConnection() => new(ConnectionString);

    public async Task Reset()
    {
        await using var connection = OpenConnection();
        await connection.ExecuteAsync("delete from event_streams");
    }

    public void Dispose() => ShutDown().Wait();

    public async ValueTask DisposeAsync() => await ShutDown();

    public Task<(string stdOut, string errorOut)> Log() =>
        container.Log();

    private async Task ShutDown()
    {
        await container.Stop();
        await container.Remove();
    }
}