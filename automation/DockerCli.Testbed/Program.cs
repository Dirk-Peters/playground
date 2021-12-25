using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerCli.Testbed;

var client = new DockerClientConfiguration().CreateClient();
var images = await client.Images.ListImagesAsync(new ImagesListParameters());
images
    .Select(i => new ImageDescription(
        i.ID,
        i.Labels.SafeSelect(p => new ImageLabel(p.Key, p.Value)).ToArray(),
        i.RepoTags.ToArray()))
    .ToList()
    .ForEach(i => Console.WriteLine(JsonSerializer.Serialize(i, new JsonSerializerOptions { WriteIndented = true })));

var container = await client.Containers.CreateContainerAsync(
    new CreateContainerParameters{Image = "hello-world"});
await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters());

using var log = await client.Containers.GetContainerLogsAsync(
    container.ID,
    false,
    new ContainerLogsParameters { Follow = true, ShowStdout = true },
    CancellationToken.None);

var result = await log.ReadOutputToEndAsync(CancellationToken.None);

Console.Error.WriteLine(result.stderr);
Console.Out.WriteLine(result.stdout);

await client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters { Force = true });

public sealed record ImageLabel(string Key, string Value);

public sealed record ImageDescription(string Id, ImageLabel[] Labels, string[] RepoTags);