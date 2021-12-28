using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using MonadicBits;

namespace DockerCli.Testbed
{
    using static Functional;

    public interface IContainerOptions
    {
        IContainerOptions Named(string name);
        IContainerOptions EnvironmentVariable(string key, string value);
        IContainerOptions MapPort(uint containerPort, uint exposedPort);
    }

    public sealed class DockerHost
    {
        private readonly DockerClient client;

        public DockerHost() =>
            client = new DockerClientConfiguration().CreateClient();

        public Task<DockerContainer> Run(ImageName image) => Run(image, _ => { });

        public async Task<DockerContainer> Run(ImageName image, Action<IContainerOptions> initializer)
        {
            var options = new ContainerOptions(image);
            initializer(options);
            var containerResponse = await client
                .Containers
                .CreateContainerAsync(options.Parameters());

            await client.Containers.StartContainerAsync(
                containerResponse.ID,
                new ContainerStartParameters());

            return new DockerContainer(containerResponse.ID, client);
        }

        private sealed class ContainerOptions : IContainerOptions
        {
            private Maybe<string> containerName = Nothing;
            private readonly ImageName imageName;
            private readonly IDictionary<string, string> environment = new Dictionary<string, string>();
            private readonly IDictionary<uint, uint> mappedPorts = new Dictionary<uint, uint>();

            public ContainerOptions(ImageName imageName)
            {
                this.imageName = imageName;
            }

            public IContainerOptions Named(string name)
            {
                containerName = name;
                return this;
            }

            public IContainerOptions EnvironmentVariable(string key, string value)
            {
                environment.Add(key, value);
                return this;
            }

            public IContainerOptions MapPort(uint containerPort, uint exposedPort)
            {
                mappedPorts.Add(containerPort, exposedPort);
                return this;
            }

            public CreateContainerParameters Parameters() =>
                new()
                {
                    Image = imageName.Full,
                    Name = containerName.Match(s => s, () => string.Empty),
                    Env = environment.Select(p => $"{p.Key}={p.Value}").ToList(),
                    ExposedPorts = mappedPorts.Values.ToDictionary(ep => $"{ep}/tcp", _ => new EmptyStruct()),
                    HostConfig = new HostConfig
                    {
                        PortBindings = mappedPorts.ToDictionary<KeyValuePair<uint, uint>, string, IList<PortBinding>>(
                            pair => $"{pair.Value}/tcp",
                            pair => new List<PortBinding> { new() { HostPort = pair.Key.ToString() } })
                    }
                };
        }
    }
}