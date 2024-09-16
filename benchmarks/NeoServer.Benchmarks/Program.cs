using System;
using BenchmarkDotNet.Running;
using NeoServer.Benchmarks.Allocations;
using NeoServer.Benchmarks.Script;

namespace NeoServer.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        //  BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugBuildConfig());
        BenchmarkRunner.Run<AreaEffectBenchmark>();

        Console.ReadKey();
    }
}