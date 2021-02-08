using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Chats;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Model;
using NeoServer.Game.DataStore;
using NeoServer.Networking.Packets.Incoming;
using Newtonsoft.Json;
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
                var rewriter = new ScriptRewriter();
                var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

                var syntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);
                var result = rewriter.Visit(syntaxTree.GetRoot());

                syntaxTrees[i++] = result.SyntaxTree;

            }

            var references = new HashSet<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(EffectT).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ICreature).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Creature).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(BaseSpell).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ChatChannel).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(ChatChannelStore).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IncomingPacket).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(InvalidOperation).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(JsonConvert).Assembly.Location),
            };
         
            foreach (var assembly in typeof(JsonConvert).Assembly.GetReferencedAssemblies())
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load(assembly.FullName).Location));
            }

            Assembly.GetEntryAssembly().GetReferencedAssemblies()
                .ToList()
                .ForEach(a => references.Add(MetadataReference.CreateFromFile(Assembly.Load(a).Location)));
            return CSharpCompilation.Create("Scripts.dll",
              syntaxTrees,
                references: references,

                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,

                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default).WithPlatform(Platform.AnyCpu));
        }
    }
}
