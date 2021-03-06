﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Compiler.Compilers
{
    public class ExtensionsMetadata
    {
        public ExtensionsMetadata(string hash, string assemblyName)
        {
            Hash = hash;
            AssemblyName = assemblyName;
        }
        public string Hash { get;  }
        public string AssemblyName { get; }

        private static readonly string MetadataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extensions.temp");
        private static string GenerateSourceHash(string[] sources)
        {
            var source = string.Join(string.Empty, sources);
            return BitConverter.ToString(MD5.HashData(Encoding.UTF8.GetBytes(source)));
        }
        public static void Save(Assembly assembly,string[] sources)
        {
            var hash = GenerateSourceHash(sources);
            var lines = new string[] {assembly.ManifestModule.ScopeName, hash};
            File.WriteAllLines(MetadataPath, lines);
        }

        public static ExtensionsMetadata Metadata
        {
            get
            {
                if(!File.Exists(MetadataPath)) return null;

                var lines = File.ReadAllLines(MetadataPath);
                return new ExtensionsMetadata(lines?.LastOrDefault(),
                    lines?.FirstOrDefault());

            }
        }
    

        public static bool SameHash(string[] sources) => Metadata?.Hash == GenerateSourceHash(sources);
    }
}
