using Autofac;
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
    private readonly IComponentContext _container;
    private readonly IGameServer _gameServer;

    private readonly IMap _map;
    private readonly SafeTradeSystem _tradeSystem;

    public EventSubscriber(IMap map, IGameServer gameServer, IComponentContext container, SafeTradeSystem tradeSystem)
    {
        _map = map;
        _gameServer = gameServer;
        _container = container;
        _tradeSystem = tradeSystem;
    }

    public void AttachEvents()
    {
        _map.OnCreatureAddedOnMap += (creature, cylinder) =>
            _container.Resolve<CreatureAddedOnMapEventHandler>().Execute(creature, cylinder);

        _map.OnCreatureAddedOnMap += (creature, _) =>
            _container.Resolve<PlayerSelfAppearOnMapEventHandler>().Execute(creature);

        _map.OnThingRemovedFromTile += _container.Resolve<ThingRemovedFromTileEventHandler>().Execute;
        _map.OnCreatureMoved += _container.Resolve<CreatureMovedEventHandler>().Execute;
        _map.OnThingMovedFailed += _container.Resolve<InvalidOperationEventHandler>().Execute;
        _map.OnThingAddedToTile += _container.Resolve<ThingAddedToTileEventHandler>().Execute;
        _map.OnThingUpdatedOnTile += _container.Resolve<ThingUpdatedOnTileEventHandler>().Execute;

        BaseSpell.OnSpellInvoked += _container.Resolve<SpellInvokedEventHandler>().Execute;

        OperationFailService.OnOperationFailed += _container.Resolve<PlayerOperationFailedEventHandler>().Execute;
        OperationFailService.OnInvalidOperation += _container.Resolve<PlayerOperationFailedEventHandler>().Execute;
        NotificationSenderService.OnNotificationSent += _container.Resolve<NotificationSentEventHandler>().Execute;
        _gameServer.OnOpened += _container.Resolve<ServerOpenedEventHandler>().Execute;

        AddTradeHandlers();
    }

    private void AddTradeHandlers()
    {
        _tradeSystem.OnClosed += _container.Resolve<TradeClosedEventHandler>().Execute;
        _tradeSystem.OnTradeRequest += _container.Resolve<TradeRequestedEventHandler>().Execute;
    }
}