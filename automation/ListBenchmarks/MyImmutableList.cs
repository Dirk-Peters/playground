using System.Collections;

namespace ListBenchmarks;

public readonly struct MyImmutableList<T> : IEnumerable<T>
{
    private readonly IBucket bucket;

    private MyImmutableList(IBucket bucket) => this.bucket = bucket;

    public IEnumerator<T> GetEnumerator() => bucket.Values().GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public MyImmutableList<T> Push(T item) => bucket.Add(item).ToList();
    public MyImmutableList<T> Popped() => bucket.Pop().ToList();
    public MyImmutableList<T> Popped(Func<T, bool> handle) => bucket.Pop(handle).ToList();

    public MyImmutableList<T> RemoveValues(T value)
    {
        var unchangedBuckets = new Stack<TailedBucket>();
        TailedBucket lastUnchangedBucket = null;
        var valueHash = value.GetHashCode();

        foreach (var tailedBucket in bucket.Buckets())
        {
            if (tailedBucket.ValueHash == valueHash)
            {
                if (lastUnchangedBucket != null)
                {
                    unchangedBuckets.Push(lastUnchangedBucket);
                }

                lastUnchangedBucket = null;
            }
            else
            {
                lastUnchangedBucket ??= tailedBucket;
            }
        }

        return unchangedBuckets.Aggregate(
                (IBucket)lastUnchangedBucket ?? new EmptyBucket(),
                (t, b) => t.Add(b.Head))
            .ToList();
    }

    public static MyImmutableList<T> Create() => new EmptyBucket().ToList();

    private interface IBucket
    {
        IEnumerable<T> Values();
        IEnumerable<TailedBucket> Buckets();
        MyImmutableList<T> ToList();
        IBucket Add(T item) => new TailedBucket(item, this, item.GetHashCode());
        IBucket Remove(T item);
        IBucket Pop();
        IBucket Pop(Func<T, bool> handle);
    }

    private sealed record EmptyBucket : IBucket
    {
        public IEnumerable<T> Values()
        {
            yield break;
        }

        public IEnumerable<TailedBucket> Buckets()
        {
            yield break;
        }

        public MyImmutableList<T> ToList() => new(this);
        public IBucket Remove(T item) => this;
        public IBucket Pop() => this;
        public IBucket Pop(Func<T, bool> handle) => Pop();
    }

    private sealed record TailedBucket(T Head, IBucket Tail, int ValueHash) : IBucket
    {
        public IEnumerable<T> Values() =>
            new ValueEnumerable(this);

        public IEnumerable<TailedBucket> Buckets() =>
            new BucketEnumerable(this);

        public MyImmutableList<T> ToList() => new(this);

        public IBucket Remove(T item) =>
            Equals(item, Head)
                ? Tail.Remove(item)
                : new TailedBucket(Head, Tail.Remove(item), ValueHash);

        public IBucket Pop() => Tail;

        public IBucket Pop(Func<T, bool> handle) =>
            handle(Head) ? Pop() : this;
    }

    private sealed class BucketEnumerable : IEnumerable<TailedBucket>
    {
        private readonly TailedBucket bucket;

        public BucketEnumerable(TailedBucket bucket) =>
            this.bucket = bucket;

        public IEnumerator<TailedBucket> GetEnumerator() =>
            new BucketEnumerator(bucket);

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }

    private sealed class BucketEnumerator : IEnumerator<TailedBucket>
    {
        private TailedBucket currentBucket;
        private bool started;
        private bool hasMore = true;

        public BucketEnumerator(TailedBucket currentBucket) =>
            this.currentBucket = currentBucket;

        public bool MoveNext()
        {
            if (!started)
                started = true;
            else if (currentBucket.Tail is TailedBucket tail)
                currentBucket = tail;
            else
                hasMore = false;

            return hasMore;
        }

        public void Reset()
        {
        }

        public TailedBucket Current => currentBucket;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }

    private sealed class ValueEnumerable : IEnumerable<T>
    {
        private readonly TailedBucket bucket;

        public ValueEnumerable(TailedBucket bucket) => this.bucket = bucket;

        public IEnumerator<T> GetEnumerator() => new ValueEnumerator(bucket);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    private sealed class ValueEnumerator : IEnumerator<T>
    {
        private TailedBucket currentBucket;
        private bool started;
        private bool hasMore = true;

        public ValueEnumerator(TailedBucket currentBucket) =>
            this.currentBucket = currentBucket;

        public bool MoveNext()
        {
            if (!started)
                started = true;
            else if (currentBucket.Tail is TailedBucket tail)
                currentBucket = tail;
            else
                hasMore = false;

            return hasMore;
        }

        public void Reset()
        {
        }

        public T Current => currentBucket.Head;

        object IEnumerator.Current => Current;

        public void Dispose() => Reset();
    }
}