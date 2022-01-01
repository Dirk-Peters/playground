
using System.Reactive.Linq;
using System.Text;
using Docker.DotNet;
using Docker.DotNet.Models;
using MonadicBits;

namespace Docker;

using static Functional;

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

    public async Task LogToConsole()
    {
        using var log = await client.Containers.GetContainerLogsAsync(
            id,
            false,
            new ContainerLogsParameters { Follow = false, ShowStdout = true },
            CancellationToken.None);
        await using var outStream = Console.OpenStandardOutput();
        await log.CopyOutputToAsync(
            Stream.Null,
            outStream,
            outStream, CancellationToken.None);
    }

    public async Task WaitFor(string message) =>
        await (await LogAndFollow())
            .Where(l => l.Contains(message))
            .FirstAsync();

    public async Task<IObservable<string>> LogAndFollow()
    {
        var log = await client.Containers.GetContainerLogsAsync(
            id,
            false,
            new ContainerLogsParameters { Follow = true, ShowStdout = true },
            CancellationToken.None);
        return Observable.Create<string>(observer =>
        {
            var reader = new AsyncLogLineReader(log);
            var cancellationSource = new CancellationTokenSource();

            bool OnNext(string s)
            {
                observer.OnNext(s);
                return true;
            }

            Task.Run(
                async () =>
                {
                    while (!cancellationSource.IsCancellationRequested)
                    {
                        if (!reader.HasMore)
                        {
                            observer.OnCompleted();
                            return;
                        }

                        var nextLine = await reader.NextLine();
                        if (!nextLine.Match(OnNext, () => false))
                        {
                            await Task.Yield();
                        }
                    }
                },
                cancellationSource.Token);
            return reader;
        });
    }

    private sealed class AsyncLogLineReader : IDisposable
    {
        private const char NewLine = '\n';
        private readonly MultiplexedStream stream;
        private readonly Queue<string> lineBuffer = new();
        private string incompleteLine;
        private bool hasMore = true;
        public bool HasMore => lineBuffer.Count > 0 || hasMore;

        public AsyncLogLineReader(MultiplexedStream stream) =>
            this.stream = stream;

        public async Task<Maybe<string>> NextLine()
        {
            if (lineBuffer.Count > 0)
            {
                return lineBuffer.Dequeue();
            }

            if (!hasMore)
            {
                return Nothing;
            }

            var localBuffer = new byte[256];
            var result = await stream.ReadOutputAsync(
                localBuffer, 0, localBuffer.Length, CancellationToken.None);

            return result switch
            {
                { EOF: false, Count: > 0 } => await TryReadNextLine(localBuffer, result.Count),
                { EOF: true } => await OnEof(),
                _ => Nothing
            };
        }

        private Task<Maybe<string>> OnEof()
        {
            hasMore = false;

            if (!string.IsNullOrEmpty(incompleteLine))
            {
                lineBuffer.Enqueue(incompleteLine);
            }

            return NextLine();
        }

        private async Task<Maybe<string>> TryReadNextLine(IEnumerable<byte> buffer, int count)
        {
            var text = Encoding.UTF8.GetString(buffer.Take(count).ToArray());
            if (text.Contains(NewLine))
            {
                var lines = text.Split(NewLine);
                lines[0] = incompleteLine + lines[0];

                if (text.EndsWith(NewLine))
                {
                    lines.SkipLast(1).ToList().ForEach(l => lineBuffer.Enqueue(l));
                }
                else
                {
                    lines.SkipLast(1)
                        .ToList()
                        .ForEach(l => lineBuffer.Enqueue(l));
                    incompleteLine = lines.Last();
                }

                return await NextLine();
            }

            incompleteLine += text;
            return Nothing;
        }

        public void Dispose() => stream.Dispose();
    }
}