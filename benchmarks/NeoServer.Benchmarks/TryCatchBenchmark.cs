using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace NeoServer.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, 100)]
public class TryCatchBenchmark
{
    [Benchmark]
    public long WithTryCatch()
    {
        var sum = 0;

        for (var i = 0; i < 1_000_000; i++)
            try
            {
                sum += i;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        return sum;
    }

    [Benchmark]
    public long WithoutTryCatch()
    {
        var sum = 0;

        for (var i = 0; i < 1_000_000; i++) sum += i;
        return sum;
    }
}