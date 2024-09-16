using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;

namespace NeoServer.Benchmarks.Allocations;

[ShortRunJob]
[MemoryDiagnoser]
public class ArrayPoolVsStackallocBenchmark
{
    private static readonly ArrayPool<byte> Pool = ArrayPool<byte>.Shared;
    
    [Benchmark]
    public void ArrayPool()
    {
        var buffer = Pool.Rent(100);
        buffer[0] = 10;
        Pool.Return(buffer);
    }

    [Benchmark]
    public void Stackalloc()
    {
        Span<byte> buffer = stackalloc byte[100];
        buffer[0] = 10;
    }

    [Benchmark(Baseline = true)]
    public void NewBuffer()
    {
        var buffer = new byte[100];
        buffer[0] = 10;
    }
}