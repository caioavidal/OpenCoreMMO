using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using NeoServer.Benchmarks.Networking;
using NeoServer.Benchmarks.World;
using System;

namespace NeoServer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //  BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
            //var summary = BenchmarkRunner.Run<BlockCopyVsSpan>();
            var summary = BenchmarkRunner.Run<GetCreaturesAtPositionZoneBenchmark>();
            Console.ReadKey();
        }
    }
}
