using DockerCli.Testbed;

var host = new DockerHost();
var container = await host.Run(new ImageName("hello-world", "latest"));
await container.Log();
await container.Remove();