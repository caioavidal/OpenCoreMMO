using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using NeoServer.Server.Attributes;
using NeoServer.Server.Compiler.Compilers;

namespace NeoServer.Server.Compiler
{
    public class ExtensionsCompiler
    {
        /// <summary>
        /// Compiles all C# files in Extensions folder
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="extensionsFolder"></param>
        /// <returns>Number of files</returns>
        public static int Compile(string basePath, string extensionsFolder)
        {
            var sourcesPath = Path.Combine(basePath, extensionsFolder);

            var compiler = new CSharpCompiler();

            var bin = Directory.GetDirectories(sourcesPath, "bin",
                new EnumerationOptions { RecurseSubdirectories = true });
            var obj = Directory.GetDirectories(sourcesPath, "obj",
                new EnumerationOptions { RecurseSubdirectories = true });

            if (bin.FirstOrDefault() is { } binFolder) Directory.Delete(binFolder, true);
            if (obj.FirstOrDefault() is { } objFolder) Directory.Delete(objFolder, true);

            var files = Directory.GetFiles(sourcesPath, "*.cs", new EnumerationOptions
            {
                AttributesToSkip = FileAttributes.Temporary,
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            });

            var sources = files.Select(File.ReadAllText).ToArray();

            if (ExtensionsMetadata.SameHash(sources) && !string.IsNullOrWhiteSpace(ExtensionsMetadata.Metadata?.AssemblyName))
            {
                ExtensionsAssembly.LoadFromDll(ExtensionsMetadata.Metadata.AssemblyName);
                return sources.Length;
            }

            var assemblyStream = compiler.Compile(sources);

            var assemblyLoaded = ExtensionsAssembly.Load(assemblyStream);

            ExtensionsMetadata.Save(assemblyLoaded, sources);
            ExtensionsAssembly.Save(assemblyLoaded, assemblyStream);

            return sources.Length;
        }

    }


}