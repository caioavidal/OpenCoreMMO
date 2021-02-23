using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Loaders
{
    public class ScriptSearch
    {
        public static IEnumerable<Type> All = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(ScriptAttribute)));

        public static T GetInstance<T>(string name, params object[] constructor)
        {
            var type = All.FirstOrDefault(x => x.Name.Equals(name));
            if (type is null) return default;

            return (T)Activator.CreateInstance(type: type, args: constructor);
        }
        public static Type Get(string name)
        {
            var type = All.FirstOrDefault(x => x.Name.Equals(name));
            return type;
        }
    }
}
