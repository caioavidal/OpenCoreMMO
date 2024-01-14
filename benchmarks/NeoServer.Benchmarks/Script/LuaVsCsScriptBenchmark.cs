using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using CSScriptLib;
using NLua;

namespace NeoServer.Benchmarks.Script;

[SimpleJob(RunStrategy.ColdStart, 1)]
public class LuaVsCsScriptBenchmark
{
    private readonly dynamic _script = CSScript.Evaluator
        .LoadMethod("""
                    void Product()
                        {
                            for(int i = 0;  i < 100; i++)
                                for(int j = 0; j < 100; j++)
                                    _ = i+j;
                        }
                    """);

    private readonly Lua _lua = new();

    [Benchmark]
    public void Lua()
    {
        for (int i = 0; i < 10_000; i++)
        {
            _ = _lua.DoString("""
                               for i = 1, 100 do
                                  for j = 1, 100 do
                                     y = i+j
                                  end
                               end
                                                             
                              """);
        }
    }


    [Benchmark]
    public void CsScript()
    {
        for (int i = 0; i < 10_000; i++)
        {
            _script.Product();
        }
    }
}