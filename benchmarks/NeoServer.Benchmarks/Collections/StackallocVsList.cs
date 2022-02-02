using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace NeoServer.Benchmarks.Collections;

[MemoryDiagnoser]
[SimpleJob(3)]
public class StackallocVsList
{
    [Benchmark]
    public List<byte> GetListBytes()
    {
        var list = new List<byte>();

        for (var i = 0; i < 1000; i++) list.Add(57);
        return list;
    }

    [Benchmark]
    public byte[] GetStackallocBytes()
    {
        Span<byte> list = stackalloc byte[1000];

        for (var i = 0; i < 1000; i++) list[i] = 57;
        return list.ToArray();
    }

    [Benchmark]
    public byte[] GetStackallocExceedingBytes()
    {
        Span<byte> list = stackalloc byte[2000];

        var count = 0;

        for (var i = 0; i < 1000; i++)
        {
            list[i] = 57;
            count++;
        }

        return list.Slice(0, count).ToArray();
    }
}