using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Features.Creature;
using NeoServer.Application.Features.Creature.Events;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class EventInjection
{
    public static IServiceCollection AddEvents(this IServiceCollection builder)
    {
        builder.RegisterServerEvents();
        builder.RegisterGameEvents();
        builder.RegisterEventSubscribers();
        builder.AddSingleton<EventSubscriber>();
        builder.AddSingleton<CreatureFactoryEventSubscriber>();

        return builder;
    }

    private static IServiceCollection RegisterServerEvents(this IServiceCollection builder)
    {
        Assembly.GetAssembly(typeof(CreatureAddedOnMapEventHandler));
        builder.RegisterAssembliesByInterface(typeof(IEventHandler));

        return builder;
    }

    private static void RegisterGameEvents(this IServiceCollection builder)
    {
        builder.RegisterAssembliesByInterface(typeof(IGameEventHandler));
    }

    private static void RegisterEventSubscribers(this IServiceCollection builder)
    {
        var types = Container.AssemblyCache;

        builder
            .RegisterAssemblyTypes<ICreatureEventSubscriber>(types)
            .RegisterAssemblyTypes<IItemEventSubscriber>(types)
            .RegisterAssemblyTypes<IChatChannelEventSubscriber>(types);
    }
}