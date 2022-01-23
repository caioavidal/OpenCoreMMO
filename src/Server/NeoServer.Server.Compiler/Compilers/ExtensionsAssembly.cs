using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NeoServer.Server.Compiler.Compilers;

internal static class ExtensionsAssembly
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static Assembly Load(byte[] compiledAssembly)
    {
        return Assembly.Load(compiledAssembly);
    }


    [MethodImpl(MethodImplOptions.NoInlining)]
    public static void LoadFromDll(string assemblyName)
    {
        if (string.IsNullOrWhiteSpace(assemblyName)) return;
        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);

        if (!File.Exists(path)) return;

        Assembly.LoadFrom(path);
    }

    public static void Save(Assembly assembly, byte[] compiledAssembly)
    {
        if (assembly?.ManifestModule?.ScopeName != null)
            File.WriteAllBytes(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.ManifestModule.ScopeName),
                compiledAssembly);
    }
}