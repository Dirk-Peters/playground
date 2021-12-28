using System;
using System.Threading.Tasks;
using DockerCli.Testbed;
using Console = System.Console;

var host = new DockerHost();
//await host.StartAndWaitForHelloWorld();
await host.StartAndWaitForPostgres();


public static class ContainerActions
{
    public static async Task StartAndLogHelloWorld(this DockerHost host)
    {
        var container = await host.Run(new ImageName("hello-world", "latest"));
        await container.Log();
        await container.Remove();
    }

    public static async Task StartAndWaitForHelloWorld(this DockerHost host)
    {
        var container = await host.Run(new ImageName("hello-world", "latest"));

        await container.WaitFor("https://docs.docker.com/engine/userguide/");

        Console.WriteLine("container reached given line");
        await container.Remove();
    }

    public static async Task StartAndWaitForPostgres(this DockerHost host)
    {
        var container = await host.Run(
            new ImageName("postgres", "latest"),
            o => o.Named("postgres-test")
                .EnvironmentVariable("POSTGRES_PASSWORD", "socrates")
                .MapPort(5432, 5432));

        await container.WaitFor("database system is ready to accept connections");
        Console.WriteLine("postgres ready");

        await Task.Delay(TimeSpan.FromSeconds(15));
        await container.Stop();
        await container.Log();

        await container.Remove();
    }
}