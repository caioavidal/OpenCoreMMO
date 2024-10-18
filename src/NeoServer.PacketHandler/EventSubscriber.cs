using Microsoft.Extensions.DependencyInjection;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Application.Server.Events;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Services;
using NeoServer.Game.Item.Items.Attributes;
using NeoServer.Modules.Combat.Events;
using NeoServer.Modules.ItemManagement.DecayManagement;
using NeoServer.Modules.Trading;
using NeoServer.Modules.Trading.CloseTrade;
using NeoServer.Modules.Trading.RequestTrade.Events;
using NeoServer.Modules.World.Events;
using NeoServer.PacketHandler.Features.Creature.Events;
using NeoServer.PacketHandler.Features.Player.Events;

namespace NeoServer.PacketHandler;

public sealed class EventSubscriber
{
    private readonly IGameServer _gameServer;
    private readonly IItemDecayTracker _itemDecayTracker;

    private readonly IMap _map;
    private readonly IServiceProvider _serviceProvider;
    private readonly SafeTradeSystem _tradeSystem;

    public EventSubscriber(IMap map, IGameServer gameServer, IServiceProvider serviceProvider,
        SafeTradeSystem tradeSystem, IItemDecayTracker itemDecayTracker)
    {
        _map = map;
        _gameServer = gameServer;
        _serviceProvider = serviceProvider;
        _tradeSystem = tradeSystem;
        _itemDecayTracker = itemDecayTracker;
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

        Decayable.OnStarted += _itemDecayTracker.Track;

        _gameServer.OnOpened += _serviceProvider.GetRequiredService<ServerOpenedEventHandler>().Execute;

        AddTradeHandlers();
    }

    private void AddTradeHandlers()
    {
        _tradeSystem.OnClosed += _serviceProvider.GetRequiredService<TradeClosedEventHandler>().Execute;
        _tradeSystem.OnTradeRequest += _serviceProvider.GetRequiredService<TradeRequestedEventHandler>().Execute;
    }
}