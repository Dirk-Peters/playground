using System.Data;
using System.Threading.Tasks;
using Spec.Common;
using Todolist;

namespace Spec.Isolation.PerTestRun;

public sealed class GlobalSystemUnderTest
{
    private PostgresDatabase? database;

    public static GlobalSystemUnderTest Instance { get; } = new();

    public void Setup() => database ??= StartDbAndMigrate().Result;
    public void ShutDown() => database?.Dispose();

    public void Reset() => database?.Reset().Wait();

    public IDbConnection? OpenConnection() => database?.OpenConnection();

    private static async Task<PostgresDatabase> StartDbAndMigrate()
    {
        var database = await SystemUnderTest.StartDatabase();

        var app = App.Setup("-m",
            $"ConnectionStrings__events=\"{database.ConnectionString}\"");

        await app.Invoke();
        return database;
    }

    public (string stdOut, string errorOut)? Log() => database?.Log().Result;
}