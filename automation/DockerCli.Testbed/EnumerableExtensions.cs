using System;
using System.Collections.Generic;
using System.Linq;

namespace DockerCli.Testbed
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<R> SafeSelect<T, R>(this IEnumerable<T> source, Func<T, R> func) => 
            source == null ? Enumerable.Empty<R>() : source.Select(func);
    }
}