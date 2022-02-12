// See https://aka.ms/new-console-template for more information


using BenchmarkDotNet.Running;
using ListBenchmarks;

//BenchmarkRunner.Run<ListBuildBenchmark>();
//BenchmarkRunner.Run<IterationBenchmark>();
BenchmarkRunner.Run(typeof(MyImmutableList<>).Assembly);