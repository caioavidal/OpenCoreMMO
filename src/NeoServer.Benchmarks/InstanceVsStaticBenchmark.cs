using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1)]

    public class InstanceVsStaticBenchmark
    {

        [Benchmark]
        public long NoMethod()
        {
            var sum = 0;
            for(int i = 0; i < 1_000; i++)
            {
                sum++;
            }
            return sum;

        }
        [Benchmark]
        public long StaticMethod()
        {

            var sum = 0;
            for (int i = 0; i < 1_000; i++)
            {
                Static.Sum(sum);
            }
            return sum;


        }
        [Benchmark]
        public long InstanceMethod()
        {
            var instance = new Instance();
            var sum = 0;
            for (int i = 0; i < 1_000; i++)
            {
                instance.Sum(sum);
            }

            return sum;
        }

    }




    public class Instance
    {

        public void Sum(int sum)
        {
            sum++;
        }

    }

    public class Static
    {
        public static void Sum( int n) => n++;

    }

}
