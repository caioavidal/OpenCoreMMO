using System;
using System.IO;

namespace NeoServer.Server.Compiler
{
    public class ScriptCompiler
    {
        public static void Compile()
        {
            var sourcesPath = Path.Combine(Environment.CurrentDirectory, "data", "scripts");

            var compiler = new Compiler();
            var runner = new Runner();

            var files = Directory.GetFiles(sourcesPath, "*.cs", SearchOption.AllDirectories);

            var assembly = compiler.Compile(files);

            foreach (var type in runner.GetAssemblies(assembly))
            {
                ScriptList.Assemblies.Add(type.Name, type);
            }
        }
    }
}
