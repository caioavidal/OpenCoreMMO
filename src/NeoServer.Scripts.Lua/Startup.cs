using System;
using System.Linq;
using NeoServer.Scripts.Lua.Patchers;
using NeoServer.Scripts.Lua.Patchers.Base;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Scripts.Lua;

public class Startup : IRunBeforeLoaders
{
    public void Run()
    {
        Patch();
    }

    private void Patch()
    {
        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
            .Where(x => x.IsAssignableTo(typeof(IPatcher)) && x.IsClass && !x.IsAbstract)
            .ToHashSet();

        foreach (var type in types)
        {
            var patch = (IPatcher)Activator.CreateInstance(type);
            patch?.Patch();
        }
    }
}