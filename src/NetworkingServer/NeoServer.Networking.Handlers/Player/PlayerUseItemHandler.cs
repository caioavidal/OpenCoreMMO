using System.Threading.Tasks;
using Mediator;
using NeoServer.Application.Features.UseItem;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player.UseItem;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerUseItemHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly PlayerUseItemCommand _playerUseItemCommand;
    private readonly HotkeyService _hotkeyService;
    private readonly IMediator _mediator;

    public PlayerUseItemHandler(IGameServer game, PlayerUseItemCommand playerUseItemCommand,
        HotkeyService hotkeyService,
        IMediator mediator)
    {
        _game = game;
        _playerUseItemCommand = playerUseItemCommand;
        _hotkeyService = hotkeyService;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemPacket = new UseItemPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var item = Find(player, useItemPacket.Location, useItemPacket.ClientId);

        if (item is IConsumable consumable)
        {
            var consumeItemCommand = new ConsumeItemCommand(Player: player, Item: consumable, Destination: player);
            _game.Dispatcher.AddEvent(new Event(2000, () => _ = ValueTask.FromResult(_mediator.Send(consumeItemCommand))));
            
            return;
        }

        _game.Dispatcher.AddEvent(new Event(2000,
            () => _playerUseItemCommand.Execute(player,
                useItemPacket))); //todo create a const for 2000 expiration time
    }

    private IItem Find(IPlayer player, Location itemLocation, ushort clientId)
    {
        if (itemLocation.IsHotkey)
        {
            return _hotkeyService.GetItem(player, clientId);
        }

        if (itemLocation.Type == LocationType.Ground)
        {
            return _game.Map[itemLocation] is not { } tile ? null : tile.TopItemOnStack;
        }

        if (itemLocation.Slot == Slot.Backpack)
        {
            var item = player.Inventory[Slot.Backpack];
            item.SetNewLocation(itemLocation);
            return item;
        }

        if (itemLocation.Type == LocationType.Container)
        {
            var item = player.Containers[itemLocation.ContainerId][itemLocation.ContainerSlot];
            item.SetNewLocation(itemLocation);
            return item;
        }

        if (itemLocation.Type == LocationType.Slot)
        {
            return player.Inventory[itemLocation.Slot];
        }

        return null;
    }
}