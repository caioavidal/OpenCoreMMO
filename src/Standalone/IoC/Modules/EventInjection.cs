using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Modules.Creatures;
using NeoServer.Modules.Creatures.Events;
using NeoServer.Modules.Party.InviteToParty;
using NeoServer.Modules.Shopping.OpenShop;
using NeoServer.Modules.World.Events;
using NeoServer.PacketHandler;

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
        //todo remove

        Assembly.GetAssembly(typeof(ThingRemovedFromTileEventHandler));
        Assembly.GetAssembly(typeof(ShowShopEventHandler));
        Assembly.GetAssembly(typeof(PlayerInviteToPartyEventHandler));
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