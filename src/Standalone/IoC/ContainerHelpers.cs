using System;
using System.Linq;
using Autofac;

namespace NeoServer.Server.Standalone.IoC;

public static class ContainerHelpers
{
    public static void RegisterAssembliesByInterface(this ContainerBuilder builder, Type interfaceType)
    {
        var types = Container.AssemblyCache.SelectMany(x => x.GetTypes())
            .Where(interfaceType.IsAssignableFrom);

        foreach (var type in types)
        {
            if (type == interfaceType) continue;
            builder.RegisterType(type).SingleInstance();
        }
    }
}