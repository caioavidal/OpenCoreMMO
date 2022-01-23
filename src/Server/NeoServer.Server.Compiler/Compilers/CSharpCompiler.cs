using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NeoServer.Server.Compiler.Compilers;

internal class CSharpCompiler
{
    private static byte[] CompileSource(params string[] sourceCodes)
    {
        using var peStream = new MemoryStream();
        var result = GenerateCode(sourceCodes).Emit(peStream);

        if (!result.Success)
            throw new Exception(string.Join("\n", result.Diagnostics.Select(x => x.GetMessage())));

        peStream.Seek(0, SeekOrigin.Begin);

        return peStream.ToArray();
    }

    public byte[] Compile(params string[] sources)
    {
        return CompileSource(sources);
    }

    private static CSharpCompilation GenerateCode(params string[] sourceCodes)
    {
        var syntaxTrees = new SyntaxTree[sourceCodes.Length];
        var i = 0;


        foreach (var source in sourceCodes)
        {
            var rewriter = new ScriptRewriter();
            var newSource = AddAttribute(source, rewriter);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

            syntaxTrees[i++] = CSharpSyntaxTree.ParseText(newSource, options);
        }

        var references = GetAssemblies()
            .Select(GetRawMetadataReference);

        return CSharpCompilation.Create("Extensions",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Release,
                allowUnsafe: true,
                assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default).WithPlatform(Platform.AnyCpu));
    }

    public static MetadataReference GetRawMetadataReference(Assembly assembly)
    {
        unsafe
        {
            return assembly.TryGetRawMetadata(out var blob, out var length)
                ? AssemblyMetadata
                    .Create(ModuleMetadata.CreateFromMetadata((IntPtr)blob, length))
                    .GetReference()
                : throw new InvalidOperationException($"Could not get raw metadata for type {assembly.GetType()}");
        }
    }

    public static List<Assembly> GetAssemblies()
    {
        var returnAssemblies = new List<Assembly>();
        var loadedAssemblies = new HashSet<string>();
        var assembliesToCheck = new Queue<Assembly>();

        assembliesToCheck.Enqueue(Assembly.GetEntryAssembly());

        while (assembliesToCheck.Any())
        {
            var assemblyToCheck = assembliesToCheck.Dequeue();

            foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
                if (!loadedAssemblies.Contains(reference.FullName))
                {
                    var assembly = Assembly.Load(reference);
                    assembliesToCheck.Enqueue(assembly);
                    loadedAssemblies.Add(reference.FullName);
                    returnAssemblies.Add(assembly);
                }
        }

        return returnAssemblies;
    }

    private static string AddAttribute(string source, ScriptRewriter rewriter)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var newNode = rewriter.Visit(syntaxTree.GetRoot());

        return newNode.ToFullString();
    }
}