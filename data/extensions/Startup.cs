using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Extensions.Services;
using NLua;

namespace NeoServer.Extensions;

public class Startup : IStartup
{
    private readonly Lua _lua;

    public Startup(Lua lua)
    {
        _lua = lua;
    }

    public void Run()
    {
        _lua["sendEffect"] = EffectService.Send;
    }
}