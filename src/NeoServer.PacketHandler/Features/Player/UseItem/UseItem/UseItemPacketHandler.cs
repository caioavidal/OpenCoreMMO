using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Domain;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Modules.ItemManagement.ContainerManagement.OpenContainer;
using NeoServer.Modules.ItemManagement.DepotManagement;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Features.Player.UseItem.UseItem;

public class PlayerUseItemPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;

    public PlayerUseItemPacketHandler(IGameServer game,
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