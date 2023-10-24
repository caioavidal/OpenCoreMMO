using System;
using System.Threading.Tasks;
using Mediator;
using NeoServer.Application.Features.UseItem;
using NeoServer.Application.Features.UseItem.UseFieldRune;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseOnItemHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly PlayerUseItemOnCommand _playerUseItemOnCommand;
    private readonly HotkeyService _hotkeyService;
    private readonly IMediator _mediator;

    public PlayerUseOnItemHandler(IGameServer game, PlayerUseItemOnCommand playerUseItemOnCommand,
        HotkeyService hotkeyService, IMediator mediator)
    {
        _game = game;
        _playerUseItemOnCommand = playerUseItemOnCommand;
        _hotkeyService = hotkeyService;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemOnPacket = new UseItemOnPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var item = GetItem(player, useItemOnPacket);
        var destination = GetDestination(player, useItemOnPacket);

        ICommand command = item switch
        {
            IConsumable consumable => new ConsumeItemCommand(player, consumable, destination),
            IFieldRune fieldRune => new UseFieldRuneCommand(player, fieldRune, useItemOnPacket.ToLocation),
            _ => null
        };

        if (command is not null)
        {
            _game.Dispatcher.AddEvent(new Event(2000, () => _ = ValueTask.FromResult(_mediator.Send(command))));
            return;
        }

        _game.Dispatcher.AddEvent(new Event(2000,
            () => _playerUseItemOnCommand.Execute(player,
                useItemOnPacket))); //todo create a const for 2000 expiration time
    }

    private IThing GetDestination(IPlayer player, UseItemOnPacket useItemPacket)
    {
        switch (useItemPacket.ToLocation.Type)
        {
            case LocationType.Ground when _game.Map[useItemPacket.ToLocation] is { } tile:
                return (IThing)tile.TopCreatureOnStack ?? tile.TopItemOnStack;

            case LocationType.Slot when
                player.Inventory[useItemPacket.ToLocation.Slot] is not null:
                return player.Inventory[useItemPacket.ToLocation.Slot];
        }

        var container = player.Containers[useItemPacket.ToLocation.ContainerId][useItemPacket.ToLocation.ContainerSlot];

        return useItemPacket.ToLocation.Type is LocationType.Container ? container : null;
    }

    private IThing GetItem(IPlayer player, UseItemOnPacket useItemPacket)
    {
        if (useItemPacket.Location.IsHotkey) return _hotkeyService.GetItem(player, useItemPacket.ClientId);

        if (useItemPacket.Location.Type is LocationType.Ground && _game.Map[useItemPacket.Location] is { } tile)
            return tile.TopItemOnStack;

        if (useItemPacket.Location.Type is LocationType.Slot &&
            player.Inventory[useItemPacket.Location.Slot] is { } item)
            return item;

        if (useItemPacket.Location.Type is LocationType.Container &&
            player.Containers[useItemPacket.Location.ContainerId][useItemPacket.Location.ContainerSlot] is
                IThing itemInContainer)
            return itemInContainer;

        return null;
    }
}