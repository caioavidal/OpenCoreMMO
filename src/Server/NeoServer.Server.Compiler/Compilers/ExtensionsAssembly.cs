using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoServer.Server.Compiler.Compilers
{
    internal class ExtensionsAssembly
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Assembly Load(byte[] compiledAssembly)
        {
            using var asm = new MemoryStream(compiledAssembly);
            var assemblyLoadContext = new SimpleUnloadableAssemblyLoadContext();

            var assembly = assemblyLoadContext.LoadFromStream(asm);

            assemblyLoadContext.Unload();
            return assembly;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LoadFromDll(string assemblyName)
        {
            if (string.IsNullOrWhiteSpace(assemblyName)) return;
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
            if (File.Exists(path)) return;

            var compiledAssembly = File.ReadAllBytes(path);
            Load(compiledAssembly);
        }
        public static void Save(Assembly assembly, byte[] compiledAssembly)
        {
            if (assembly?.ManifestModule?.ScopeName != null)
            {
                File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.ManifestModule.ScopeName),
                    compiledAssembly);
            }
        }
    }
}