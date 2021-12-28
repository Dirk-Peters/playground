using System;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerCli.Testbed
{
    public sealed class DockerContainer
    {
        private readonly string id;
        private readonly DockerClient client;

        public DockerContainer(string id, DockerClient client)
        {
            this.id = id;
            this.client = client;
        }

        public async Task Stop() =>
            await client.Containers.StopContainerAsync(id, new ContainerStopParameters());

        public async Task Remove() =>
            await client.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true });

        public async Task Log()
        {
            using var log = await client.Containers.GetContainerLogsAsync(
                id,
                false,
                new ContainerLogsParameters { Follow = true, ShowStdout = true },
                CancellationToken.None);

            var result = await log.ReadOutputToEndAsync(CancellationToken.None);

            await Console.Error.WriteLineAsync(result.stderr);
            await Console.Out.WriteLineAsync(result.stdout);
        }
    }
}