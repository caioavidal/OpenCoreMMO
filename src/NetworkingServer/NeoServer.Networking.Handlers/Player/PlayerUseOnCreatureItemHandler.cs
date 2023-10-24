using System.Threading.Tasks;
using Mediator;
using NeoServer.Application.Features.UseItem;
using NeoServer.Application.Features.UseItem.UseFieldRune;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseOnCreatureHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly PlayerUseItemOnCreatureCommand _playerUseItemOnCreatureCommand;
    private readonly HotkeyService _hotkeyService;
    private readonly IMediator _mediator;

    public PlayerUseOnCreatureHandler(IGameServer game,
        PlayerUseItemOnCreatureCommand playerUseItemOnCreatureCommand, HotkeyService hotkeyService, IMediator mediator)
    {
        _game = game;
        _playerUseItemOnCreatureCommand = playerUseItemOnCreatureCommand;
        _hotkeyService = hotkeyService;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemOnPacket = new UseItemOnCreaturePacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (!_game.CreatureManager.TryGetCreature(useItemOnPacket.CreatureId, out var creature)) return;

        var item = GetItem(player, useItemOnPacket);

        ICommand command = item switch
        {
            IConsumable consumable => new ConsumeItemCommand(player, consumable, creature),
            IFieldRune fieldRune => new UseFieldRuneCommand(player, fieldRune, creature.Location),
            _ => null
        };

        if (command is not null)
        {
            _game.Dispatcher.AddEvent(new Event(2000, () => _ = ValueTask.FromResult(_mediator.Send(command))));
            return;
        }

        _game.Dispatcher.AddEvent(new Event(2000,
            () => _playerUseItemOnCreatureCommand.Execute(player,
                useItemOnPacket))); //todo create a const for 2000 expiration time
    }

    private IThing GetItem(IPlayer player, UseItemOnCreaturePacket useItemPacket)
    {
        if (useItemPacket.FromLocation.IsHotkey) return _hotkeyService.GetItem(player, useItemPacket.ClientId);

        if (useItemPacket.FromLocation.Type is LocationType.Ground && _game.Map[useItemPacket.FromLocation] is { } tile)
            return tile.TopItemOnStack;

        if (useItemPacket.FromLocation.Type is LocationType.Slot &&
            player.Inventory[useItemPacket.FromLocation.Slot] is { } item)
            return item;

        if (useItemPacket.FromLocation.Type is LocationType.Container &&
            player.Containers[useItemPacket.FromLocation.ContainerId][useItemPacket.FromLocation.ContainerSlot] is
                IThing itemInContainer)
            return itemInContainer;

        return null;
    }
}