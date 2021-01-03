using System.Collections.Generic;
using System.Reflection;

namespace NeoServer.Server.Compiler
{
    public class ScriptList
    {
        public static Dictionary<string, TypeInfo> Assemblies { get; set; } = new Dictionary<string, TypeInfo>();
    }
}