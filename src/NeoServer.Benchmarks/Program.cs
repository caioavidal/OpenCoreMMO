using BenchmarkDotNet.Running;
using NeoServer.Benchmarks.Collections;
using System;

namespace NeoServer.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            //  BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
            //var summary = BenchmarkRunner.Run<BlockCopyVsSpan>();
            //var summary = BenchmarkRunner.Run<JobQueueBenchmark>();
            //var summary = BenchmarkRunner.Run<SchedulerQueueBenchmark>();
            //var summary = BenchmarkRunner.Run<ArrayPoolVsDynamicArrayBenchmark>();
            //var summary = BenchmarkRunner.Run<InstanceVsStaticBenchmark>();
            var summary = BenchmarkRunner.Run<StringCalculationBenchmark>();

            Console.ReadKey();
        }
    }
}
