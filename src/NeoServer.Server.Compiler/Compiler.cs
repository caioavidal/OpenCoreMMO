using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NeoServer.Server.Compiler
{
    internal class Compiler
    {
        public byte[] CompileSource(params string[] sourceCodes)
        {
            using (var peStream = new MemoryStream())
            {
                var result = GenerateCode(sourceCodes).Emit(peStream: peStream);

                if (!result.Success)
                {
                    throw new Exception(string.Join("\n", result.Diagnostics.Select(x => x.GetMessage())));
                }

                peStream.Seek(0, SeekOrigin.Begin);

                return peStream.ToArray();
            }
        }
        public byte[] Compile(params string[] filepaths)
        {
            var sources = filepaths.Select(x => File.ReadAllText(x)).ToArray();
            return CompileSource(sources);
        }

        private static CSharpCompilation GenerateCode(params string[] sourceCodes)
        {
            var syntaxTrees = new SyntaxTree[sourceCodes.Length];
            var i = 0;

            foreach (var source in sourceCodes)
            {
                var codeString = SourceText.From(source);
                var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp8);

                syntaxTrees[i++] = SyntaxFactory.ParseSyntaxTree(codeString, options);
            }

            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(EffectT).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICreature).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Creature).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BaseSpell).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(InvalidOperation).Assembly.Location)


            };

            Assembly.GetEntryAssembly().GetReferencedAssemblies()
                .ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));

            return CSharpCompilation.Create("Scripts.dll",
              syntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,

                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default).WithPlatform(Platform.AnyCpu));
        }
    }

}
