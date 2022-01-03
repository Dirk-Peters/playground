using Docker;

namespace Spec.Common;

public static class SystemUnderTest
{
    public static async Task<PostgresDatabase> StartDatabase()
    {
        var host = new DockerHost();
        var container = await host.Run(
            new ImageName("postgres", "latest"),
            o => o.Named("postgres-test")
                .EnvironmentVariable("POSTGRES_PASSWORD", "socrates")
                .MapPort(5432, 5432));
        await container.WaitFor("database system is ready to accept connections");
        return new PostgresDatabase(container, new PostgresConnectionString(5432, "socrates", "postgres"));
    }
}