using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Server.Attributes;

namespace NeoServer.Loaders;

public static class ScriptSearch
{
    public static IEnumerable<Type> All => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
        .Where(x => x.CustomAttributes.Any(customAttribute =>
            customAttribute.AttributeType == typeof(ExtensionAttribute)));

    public static T GetInstance<T>(string name, params object[] constructor)
    {
        var type = All.FirstOrDefault(x => x.Name.Equals(name));
        if (type is null) return default;

        return (T)Activator.CreateInstance(type, constructor);
    }

    public static Type Get(string name)
    {
        var type = All.FirstOrDefault(x => x.Name.Equals(name));
        return type;
    }
}