using BenchmarkDotNet.Running;
using Benchmarks;

#if DEBUG
var summary = BenchmarkRunner.Run<ReadLinesBenchmarks>(new BenchmarkDotNet.Configs.DebugInProcessConfig());
#else
            var summary = BenchmarkRunner.Run<ReadLinesBenchmarks>();
#endif

