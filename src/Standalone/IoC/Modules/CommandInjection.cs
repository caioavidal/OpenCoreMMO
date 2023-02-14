using Autofac;
using NeoServer.Server.Commands.Movements.ToContainer;
using NeoServer.Server.Commands.Movements.ToInventory;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class CommandInjection
{
    public static ContainerBuilder AddCommands(this ContainerBuilder builder)
    {
        builder.RegisterType<MapToContainerMovementOperation>().SingleInstance();
        builder.RegisterType<MapToInventoryMovementOperation>().SingleInstance();
        return builder;
    }
}