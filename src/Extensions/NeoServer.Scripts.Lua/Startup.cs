using System;
using System.Linq;
using NeoServer.Scripts.Lua.Patchers.Base;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Helpers.Extensions;
using Serilog;

namespace NeoServer.Scripts.Lua;

public class Startup : IRunBeforeLoaders
{
    private readonly ILogger _logger;

    public Startup(ILogger logger)
    {
        _logger = logger;
    }

    public void Run()
    {
        Patch();
    }

    private void Patch()
    {
        _logger.Step("Patching classes...", "Classes patched", () =>
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().AsParallel().SelectMany(x => x.GetTypes())
                .Where(x => x.IsAssignableTo(typeof(IPatcher)) && x.IsClass && !x.IsAbstract)
                .ToHashSet();

            types.AsParallel().ForAll(type =>
            {
                var patch = (IPatcher)Activator.CreateInstance(type);
                patch?.Patch();
            });
        });
    }
}