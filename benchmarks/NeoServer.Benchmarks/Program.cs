using System;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

namespace NeoServer.Benchmarks
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugBuildConfig());
            // var summary = BenchmarkRunner.Run<ReadOnlySpanVsSpanAllocBenchmark>();

            Console.ReadKey();
        }
    }
}