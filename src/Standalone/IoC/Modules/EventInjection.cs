using System.Reflection;
using Autofac;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Subscribers;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class EventInjection
{
    public static ContainerBuilder AddEvents(this ContainerBuilder builder)
    {
        builder.RegisterServerEvents();
        builder.RegisterGameEvents();
        builder.RegisterEventSubscribers();
        builder.RegisterType<EventSubscriber>().SingleInstance();
        builder.RegisterType<FactoryEventSubscriber>().SingleInstance();

        return builder;
    }

    private static void RegisterServerEvents(this ContainerBuilder builder)
    {
        var assembly = Assembly.GetAssembly(typeof(CreatureAddedOnMapEventHandler));
        builder.RegisterAssemblyTypes(assembly);
    }

    private static void RegisterGameEvents(this ContainerBuilder builder)
    {
        builder.RegisterAssembliesByInterface(typeof(IGameEventHandler));
    }

    private static void RegisterEventSubscribers(this ContainerBuilder builder)
    {
        var types = Container.AssemblyCache;
        builder.RegisterAssemblyTypes(types).As<ICreatureEventSubscriber>().SingleInstance();
        builder.RegisterAssemblyTypes(types).As<IItemEventSubscriber>().SingleInstance();
        builder.RegisterAssemblyTypes(types).As<IChatChannelEventSubscriber>().SingleInstance();
    }
}