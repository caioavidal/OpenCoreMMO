using Mediator;
using NeoServer.Application.Features.Player;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Commands.Player;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerThrowItemHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly PlayerThrowItemCommand _playerThrowItemCommand;
    private readonly IMediator _mediator;
    private readonly ItemFinder _itemFinder;

    public PlayerThrowItemHandler(IGameServer game, PlayerThrowItemCommand playerThrowItemCommand, IMediator mediator,
        ItemFinder itemFinder)
    {
        _game = game;
        _playerThrowItemCommand = playerThrowItemCommand;
        _mediator = mediator;
        _itemFinder = itemFinder;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var itemThrowPacket = new ItemThrowPacket(message);
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (itemThrowPacket.ToLocation.Type is LocationType.Slot &&
            itemThrowPacket.ToLocation.Slot is not Slot.Backpack)
        {
            var item = _itemFinder.Find(player, itemThrowPacket.FromLocation, itemThrowPacket.ItemClientId);

            if (item is null) return;

            var source = item.Parent ?? _game.Map[item.Location];

            var command = new DressEquipmentCommand(player, item, source as IHasItem,
                itemThrowPacket.FromStackPosition,
                itemThrowPacket.Count, itemThrowPacket.ToLocation.Slot);

            _game.Dispatcher.AddEvent(new Event(2000, () => _mediator.Send(command)));

            return;
        }

        _game.Dispatcher.AddEvent(new Event(2000,
            () => _playerThrowItemCommand.Execute(player,
                itemThrowPacket))); //todo create a const for 2000 expiration time
    }
}