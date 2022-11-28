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

        var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assemblyName);
        var pdbPath = Path.ChangeExtension(dllPath, "pdb");

        if (!File.Exists(dllPath)) return;
        var dll = File.ReadAllBytes(dllPath);

        byte[] pdb = null;
        if (File.Exists(pdbPath)) pdb = File.ReadAllBytes(pdbPath);

        Assembly.Load(dll, pdb);
    }

    public static void Save(Assembly assembly, byte[] compiledAssembly, byte[] symbolsStream)
    {
        if (assembly?.ManifestModule?.ScopeName == null) return;

        var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.ManifestModule.ScopeName);
        var pdbPath = Path.ChangeExtension(dllPath, "pdb");

        File.WriteAllBytes(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, assembly.ManifestModule.ScopeName),
            compiledAssembly);

        File.WriteAllBytes(
            pdbPath,
            symbolsStream);
    }
}