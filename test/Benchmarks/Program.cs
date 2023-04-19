using BenchmarkDotNet.Running;
using Benchmarks;

#if DEBUG
var summary = BenchmarkRunner.Run<Serialization>(new BenchmarkDotNet.Configs.DebugInProcessConfig());
#else
            var summary = BenchmarkRunner.Run<Serialization>();
#endif

