using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Application.Features.Combat.Events;
using NeoServer.Application.Features.Creature.Events;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Tasks;
using NeoServer.Server.Events.Subscribers;

namespace NeoServer.Server.Standalone.IoC.Modules;

public static class EventInjection
{
    public static IServiceCollection AddEvents(this IServiceCollection builder)
    {
        builder.RegisterServerEvents();
        builder.RegisterGameEvents();
        builder.RegisterEventSubscribers();
        builder.AddSingleton<EventSubscriber>();
        builder.AddSingleton<FactoryEventSubscriber>();

        return builder;
    }

    private static IServiceCollection RegisterServerEvents(this IServiceCollection builder)
    {
        var assembly = Assembly.GetAssembly(typeof(CreatureAddedOnMapEventHandler));
        //builder.RegisterAssemblyTypes(assembly);

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
            .RegisterAssemblyTypes<IChatChannelEventSubscriber>(types);
    }
}