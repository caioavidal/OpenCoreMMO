using System;
using System.Buffers;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;

namespace NeoServer.Benchmarks.Collections;

[SimpleJob(RunStrategy.ColdStart, 100)]
[MemoryDiagnoser]
public class ArrayPoolVsDynamicArrayBenchmark
{
    [Benchmark]
    public byte[] GetBytesUsingPool()
    {
        var pool = ArrayPool<byte>.Shared;
        var data = pool.Rent(1_00_000);

        for (var i = 0; i < 1_00_000; i++) data[i] = default;

        var streamResult = data.AsSpan(0, data.Length).ToArray();

        pool.Return(data, true);
        return streamResult;
    }

    [Benchmark]
    public byte[] GetBytes()
    {
        var data = new List<byte>();

        for (var i = 0; i < 1_00_000; i++) data.Add(default);
        return data.ToArray();
    }
}