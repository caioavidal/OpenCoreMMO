using Autofac;
using NeoServer.Networking.Handlers.ClientVersion;
using NeoServer.Networking.Listeners;
using NeoServer.Networking.Protocols;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class NetworkInjection
{
    public static ContainerBuilder AddNetwork(this ContainerBuilder builder)
    {
        builder.RegisterType<LoginProtocol>().SingleInstance();
        builder.RegisterType<GameProtocol>().SingleInstance();
        builder.RegisterType<LoginListener>().SingleInstance();
        builder.RegisterType<GameListener>().SingleInstance();
        builder.RegisterType<ClientProtocolVersion>().SingleInstance();
        return builder;
    }
}