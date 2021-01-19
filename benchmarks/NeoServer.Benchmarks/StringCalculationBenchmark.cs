using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using NeoServer.Game.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(RunStrategy.ColdStart, launchCount: 1)]
    public class StringCalculationBenchmark
    {
        [Benchmark]
        public double CalculationUsingDataTable()
        {
            double a = 0;
            for (int i = 0; i < 10000; i++)
            {
                a += StringCalculation.Calculate("a+b+c+d", ("a", i), ("b", i + 1), ("c", i + 2), ("d", i + 3));

            }
            return a;
        }
        [Benchmark]

        public double CalculationUsingRoslyn()
        {
            double a = 0;

            for (int i = 0; i < 10000; i++)
            {
                a += StringCalculation.Calculate2("a+b+c+d", ("a", i), ("b", i + 1), ("c", i + 2), ("d", i + 3));
            }

            return a;
        }
    }
}
