using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace NeoServer.Server.Compiler.Compilers
{
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

            var rewriter = new ScriptRewriter();

            foreach (var source in sourceCodes)
            {
                var newSource = AddAttribute(source, rewriter);
                var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

                syntaxTrees[i++] = CSharpSyntaxTree.ParseText(newSource, options);
            }

            var references = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(a => !a.IsDynamic)
                .Select(a => a.Location)
                .Where(s => !string.IsNullOrEmpty(s))
                .Where(s => !s.Contains("xunit"))
                .Select(s => MetadataReference.CreateFromFile(s))
                .ToList();

            AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                ?.SelectMany(x => x.GetReferencedAssemblies())
                .Where(a => a.Name?.Contains("NeoServer") ?? false)
                .ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            return CSharpCompilation.Create("Extensions",
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    allowUnsafe:true,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default).WithPlatform(Platform.AnyCpu));
        }

        private static string AddAttribute(string source, ScriptRewriter rewriter)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);

            var newNode = rewriter.Visit(syntaxTree.GetRoot());

            return newNode.ToFullString();
        }
    }
}