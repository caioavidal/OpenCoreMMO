using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace NeoServer.Benchmarks.Collections;

[MemoryDiagnoser]
[SimpleJob(10)]
public class StackVsListInsertZeroIndex
{
    private static readonly int Length = 20;

    [Benchmark]
    public int[] UsingList()
    {
        var list = new List<int>();
        for (var i = 0; i < Length; i++) list.Insert(0, i);

        return list.ToArray();
    }

    [Benchmark]
    public int[] UsingStack()
    {
        var stack = new Stack<int>();
        for (var i = 0; i < Length; i++) stack.Push(i);

        return stack.ToArray();
    }
}