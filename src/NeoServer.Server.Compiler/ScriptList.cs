using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NeoServer.Server.Compiler
{
    public class ScriptList
    {
        public static Dictionary<string, TypeInfo> Assemblies { get; set; } = new Dictionary<string, TypeInfo>();
    }
}
