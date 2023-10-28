using Mediator;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Features.Item.Depot;
using NeoServer.Application.Features.Shared;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Player.UseItem.UseItem;

public class PlayerUseItemHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;

    public PlayerUseItemHandler(IGameServer game,
        ItemFinder itemFinder,
        IMediator mediator)
    {
        _game = game;
        _itemFinder = itemFinder;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemPacket = new UseItemPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var item = _itemFinder.Find(player, useItemPacket.Location, useItemPacket.ClientId);

        item.SetNewLocation(useItemPacket.Location);

        ICommand command = item switch
        {
            IConsumable consumable => new ConsumeItemCommand(player, consumable, player),
            IDepot depot => new OpenDepotCommand(player, depot, useItemPacket.Location, useItemPacket.Index),
            IContainer container => new OpenContainerCommand(player, container, useItemPacket.Index),
            _ => new UseItemCommand(player, item)
        };

        _game.Dispatcher.AddEvent(new Event(2000, () => _ = ValueTask.FromResult(_mediator.Send(command))));
    }
}