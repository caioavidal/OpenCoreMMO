using Autofac;
using NeoServer.Scripts.Lua;
using NLua;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class LuaInjection
{
    public static ContainerBuilder AddLua(this ContainerBuilder builder)
    {
        builder.RegisterInstance(new Lua()).SingleInstance();
        builder.RegisterType<LuaGlobalRegister>().SingleInstance();
        return builder;
    }
}