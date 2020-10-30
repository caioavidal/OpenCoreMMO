using System;
using System.IO;
using System.Reactive.Linq;
namespace NeoServer.Server.Compiler
{
    public class ScriptCompiler
    {
        public static void Compile()
        {

            var sourcesPath = Path.Combine("./data/scripts/NeoServer.Scripts/Spells");


            var compiler = new Compiler();
            var runner = new Runner();

            foreach (var type in runner.GetAssemblies(compiler.Compile($"{sourcesPath}/Haste.cs")))
            {
                ScriptList.Assemblies.Add(type.Name, type);
            }
        }
    }
}
