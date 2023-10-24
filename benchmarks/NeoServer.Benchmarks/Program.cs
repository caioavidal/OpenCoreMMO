using System;
using BenchmarkDotNet.Running;
using NeoServer.Benchmarks.DIFramework;

namespace NeoServer.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        //  BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugBuildConfig());
        BenchmarkRunner.Run<AutoFacVsMicrosoft>();

        Console.ReadKey();
    }
}