using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace NeoServer.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(RunStrategy.ColdStart, 100)]
public class InstanceVsStaticBenchmark
{
    [Benchmark]
    public long NoMethod()
    {
        var sum = 0;
        for (var i = 0; i < 1_00000; i++) sum = i + i;
        return sum;
    }

    [Benchmark]
    public long StaticMethod()
    {
        var sum = 0;
        for (var i = 0; i < 1_00000; i++) sum = Static.Sum(i, i);
        return sum;
    }

    [Benchmark]
    public long InstanceMethod()
    {
        var sum = 0;
        for (var i = 0; i < 1_00000; i++)
        {
            var instance = new Instance(1, 1);
            sum = instance.Sum();
        }

        return sum;
    }
}

public class Instance
{
    private readonly int a;
    private readonly int b;

    public Instance(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    public int Sum()
    {
        return a + b;
    }
}

public class Static
{
    public static int Sum(int a, int b)
    {
        return a + b;
    }
}