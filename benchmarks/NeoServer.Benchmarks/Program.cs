using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using NeoServer.Benchmarks.Allocations;
using NeoServer.Benchmarks.Collections;
using NeoServer.Benchmarks.Tasks;
using System;

namespace NeoServer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugBuildConfig());
           // var summary = BenchmarkRunner.Run<ReadOnlySpanVsSpanAllocBenchmark>();

            Console.ReadKey();
        }
    }
}
