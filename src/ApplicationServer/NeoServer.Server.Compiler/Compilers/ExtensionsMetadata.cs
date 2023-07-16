using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NeoServer.Server.Compiler.Compilers;

public class ExtensionsMetadata
{
    private static readonly string
        MetadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extensions.temp");

    private ExtensionsMetadata(string hash, string assemblyName)
    {
        Hash = hash;
        AssemblyName = assemblyName;
    }

    private string Hash { get; }
    public string AssemblyName { get; }

    public static ExtensionsMetadata Metadata
    {
        get
        {
            if (!File.Exists(MetadataPath)) return null;

            var lines = File.ReadAllLines(MetadataPath);
            return new ExtensionsMetadata(lines?.LastOrDefault(),
                lines?.FirstOrDefault());
        }
    }

    private static string GenerateSourceHash(string[] sources)
    {
        var source = string.Concat(sources);
        return BitConverter.ToString(MD5.HashData(Encoding.UTF8.GetBytes(source)));
    }

    public static void Save(Assembly assembly, string[] sources)
    {
        var hash = GenerateSourceHash(sources);
        var lines = new[] { assembly.ManifestModule.ScopeName, hash };
        File.WriteAllLines(MetadataPath, lines);
    }

    public static bool SameHash(string[] sources)
    {
        return Metadata?.Hash == GenerateSourceHash(sources);
    }
}