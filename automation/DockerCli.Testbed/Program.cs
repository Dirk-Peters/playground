using System;
using System.Threading.Tasks;
using DockerCli.Testbed;
using Console = System.Console;

var host = new DockerHost();
var container = await host.Run(new ImageName("hello-world", "latest"));
var isRunning = true;
var log = await container.LogAndFollow();

using var subscription = log.Subscribe(
    Console.WriteLine,
    error => Console.Error.WriteLine(error),
    () => isRunning = false);

while (isRunning)
{
    await Task.Yield();
}

await container.Remove();
await Console.Out.FlushAsync();