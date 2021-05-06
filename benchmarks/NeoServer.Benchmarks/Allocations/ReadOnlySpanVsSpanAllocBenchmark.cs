using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Benchmarks.Allocations
{
    [SimpleJob(RunStrategy.ColdStart, launchCount: 50)]

    [MemoryDiagnoser]
    public class ReadOnlySpanVsSpanAllocBenchmark
    {
        [Benchmark]
        public byte UsingVar()
        {
            var array = new byte[] { 1, 2, 3, 4 };
            return array[0];
        }
        [Benchmark]
        public byte UsingReadOnlySpan()
        {
            ReadOnlySpan<byte> array = new byte[] { 1, 2, 3, 4 };
            return array[0];
        }
        [Benchmark]
        public byte UsingSpan()
        {
            Span<byte> array = new byte[] { 1, 2, 3, 4 };
            return array[0];
        }
        [Benchmark]
        public byte UsingSpanWithStackalloc()
        {
            Span<byte> array = stackalloc byte[] { 1, 2, 3, 4 };
            return array[0];
        }
    }
}
