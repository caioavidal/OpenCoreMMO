using System;
using System.IO;

namespace NeoServer.Server.Compiler
{
    public class ScriptCompiler
    {
        public static void Compile(string basePath)
        {
            var sourcesPath = Path.Combine(basePath, "scripts");

            var compiler = new Compiler();
            var runner = new Runner();

            var files = Directory.GetFiles(sourcesPath, "*.cs", new EnumerationOptions
            {
                 AttributesToSkip = FileAttributes.Temporary,
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            });

            var assembly = compiler.Compile(files);

            foreach (var type in runner.GetAssemblies(assembly))
            {
                ScriptList.Assemblies.Add(type.Name, type);
            }
        }
    }
}
