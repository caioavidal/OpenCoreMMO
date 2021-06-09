using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoServer.Server.Compiler
{
    internal class Runner
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static IEnumerable<TypeInfo> LoadAndExecute(byte[] compiledAssembly)
        {
            using (var asm = new MemoryStream(compiledAssembly))
            {
                var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

                var assembly = assemblyLoadContext.LoadFromStream(asm);

                assemblyLoadContext.Unload();
                return assembly.DefinedTypes;
            }
        }
    }
}