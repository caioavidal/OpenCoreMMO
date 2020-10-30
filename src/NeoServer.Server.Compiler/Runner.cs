using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace NeoServer.Server.Compiler
{
    internal class Runner
    {
        public void Execute(byte[] compiledAssembly, params string[] args)
        {
            var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly);
        }
        public IEnumerable<TypeInfo> GetAssemblies(byte[] compiledAssembly)
        {
            return LoadAndExecute(compiledAssembly);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IEnumerable<TypeInfo> LoadAndExecute(byte[] compiledAssembly)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                var assembly = assemblyLoadContext.LoadFromStream(asm);

                var entry = assembly.EntryPoint;
                var assemblies = assembly.DefinedTypes;

                assemblyLoadContext.Unload();
                return assemblies;
            }
        }
    }
}
