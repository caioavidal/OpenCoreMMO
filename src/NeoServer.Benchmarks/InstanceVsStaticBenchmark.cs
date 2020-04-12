using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 50)]

    public class InstanceVsStaticBenchmark
    {
        [Benchmark]
        public Dictionary<int, int> StaticMethod()
        {

            return Static.Sum(Enumerable.Range(0, 1000_000).ToArray());



        }
        [Benchmark]
        public Dictionary<int,int> InstanceMethod()
        {

            return new Instance(Enumerable.Range(0, 1000_000).ToArray()).dicNumbers;

        }

    }

    public class Instance
    {
        public Dictionary<int, int> dicNumbers = new Dictionary<int, int>();

        public Instance(int[] numbers)
        {
            foreach (var n in numbers)
            {
                dicNumbers.Add(n, n);
            }
        }

    }

    public class Static
    {
        public static Dictionary<int, int> Sum(int[] numbers)
        {
            var dicNumbers = new Dictionary<int, int>();

            foreach (var n in numbers)
            {
                dicNumbers.Add(n, n);
            }
            return dicNumbers;
        }

    }

}
