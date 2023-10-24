using System;
using Microsoft.Extensions.DependencyInjection;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Systems.SafeTrade;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Events.Combat;
using NeoServer.Server.Events.Creature;
using NeoServer.Server.Events.Player;
using NeoServer.Server.Events.Player.Trade;
using NeoServer.Server.Events.Server;
using NeoServer.Server.Events.Tiles;

namespace NeoServer.Server.Events.Subscribers;

public sealed class EventSubscriber
{
    private readonly IGameServer _gameServer;
    private readonly IServiceProvider _serviceProvider;

    private readonly IMap _map;
    private readonly SafeTradeSystem _tradeSystem;

    public EventSubscriber(IMap map, IGameServer gameServer, IServiceProvider serviceProvider,
        SafeTradeSystem tradeSystem)
    {
        _map = map;
        _gameServer = gameServer;
        _serviceProvider = serviceProvider;
        _tradeSystem = tradeSystem;
    }

    public void AttachEvents()
    {
        _map.OnCreatureAddedOnMap += (creature, cylinder) =>
            _serviceProvider.GetRequiredService<CreatureAddedOnMapEventHandler>().Execute(creature, cylinder);

        _map.OnCreatureAddedOnMap += (creature, _) =>
            _serviceProvider.GetRequiredService<PlayerSelfAppearOnMapEventHandler>().Execute(creature);

        _map.OnThingRemovedFromTile += _serviceProvider.GetRequiredService<ThingRemovedFromTileEventHandler>().Execute;
        _map.OnCreatureMoved += _serviceProvider.GetRequiredService<CreatureMovedEventHandler>().Execute;
        _map.OnThingMovedFailed += _serviceProvider.GetRequiredService<InvalidOperationEventHandler>().Execute;
        _map.OnThingAddedToTile += _serviceProvider.GetRequiredService<ThingAddedToTileEventHandler>().Execute;
        _map.OnThingUpdatedOnTile += _serviceProvider.GetRequiredService<ThingUpdatedOnTileEventHandler>().Execute;

        BaseSpell.OnSpellInvoked += _serviceProvider.GetRequiredService<SpellInvokedEventHandler>().Execute;

        OperationFailService.OnOperationFailed +=
            _serviceProvider.GetRequiredService<PlayerOperationFailedEventHandler>().Execute;

        OperationFailService.OnInvalidOperation +=
            _serviceProvider.GetRequiredService<PlayerOperationFailedEventHandler>().Execute;

        NotificationSenderService.OnNotificationSent +=
            _serviceProvider.GetRequiredService<NotificationSentEventHandler>().Execute;

        _gameServer.OnOpened += _serviceProvider.GetRequiredService<ServerOpenedEventHandler>().Execute;

        AddTradeHandlers();
    }

    private void AddTradeHandlers()
    {
        _tradeSystem.OnClosed += _serviceProvider.GetRequiredService<TradeClosedEventHandler>().Execute;
        _tradeSystem.OnTradeRequest += _serviceProvider.GetRequiredService<TradeRequestedEventHandler>().Execute;
    }
}