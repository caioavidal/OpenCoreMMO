using System.IO;

namespace NeoServer.Server.Compiler
{
    public class ScriptCompiler
    {
        public static void Compile(string basePath)
        {
            var sourcesPath = Path.Combine(basePath, "scripts");

            var compiler = new Compiler();

            var files = Directory.GetFiles(sourcesPath, "*.cs", new EnumerationOptions
            {
                 AttributesToSkip = FileAttributes.Temporary,
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            });

            var assembly = compiler.Compile(files);

            Runner.LoadAndExecute(assembly);
        }
    }
}
