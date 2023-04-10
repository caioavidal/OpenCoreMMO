using System.IO;
using System.Linq;
using NeoServer.Server.Compiler.Compilers;

namespace NeoServer.Server.Compiler;

public static class ExtensionsCompiler
{
    /// <summary>
    ///     Compiles all C# files in Extensions folder
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="extensionsFolder"></param>
    /// <returns>Number of files</returns>
    public static int Compile(string basePath, string extensionsFolder)
    {
        var sourcesPath = Path.Combine(basePath, extensionsFolder);

        var files = Directory.GetFiles(sourcesPath, "*.cs", new EnumerationOptions
        {
            AttributesToSkip = FileAttributes.Temporary,
            IgnoreInaccessible = true,
            RecurseSubdirectories = true
        }).AsParallel().Where(file => !file.Contains("\\bin\\") && !file.Contains("\\obj\\"));

        var sources = files.Select(file => new Source(file, File.ReadAllText(file))).ToArray();
        var sourceCodes = sources.Select(x => x.Code).ToArray();

        if (ExtensionsMetadata.SameHash(sourceCodes) &&
            !string.IsNullOrWhiteSpace(ExtensionsMetadata.Metadata?.AssemblyName))
        {
            ExtensionsAssembly.LoadFromDll(ExtensionsMetadata.Metadata.AssemblyName);
            return sourceCodes.Length;
        }

        var (assemblyLoaded, assemblyStream, symbolsStream) = CSharpCompiler.Compile(sources);

        ExtensionsMetadata.Save(assemblyLoaded, sources.Select(x => x.Code).ToArray());
        ExtensionsAssembly.Save(assemblyLoaded, assemblyStream, symbolsStream);

        return sources.Length;
    }
}