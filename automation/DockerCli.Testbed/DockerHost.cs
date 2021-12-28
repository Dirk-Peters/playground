using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace DockerCli.Testbed
{
    public sealed class DockerHost
    {
        private readonly DockerClient client;

        public DockerHost() =>
            client = new DockerClientConfiguration().CreateClient();

        public async Task<DockerContainer> Run(ImageName image)
        {
            var containerResponse = await client.Containers.CreateContainerAsync(
                new CreateContainerParameters { Image = image.Full });

            await client.Containers.StartContainerAsync(
                containerResponse.ID,
                new ContainerStartParameters());

            return new DockerContainer(containerResponse.ID, client);
        }
    }
}