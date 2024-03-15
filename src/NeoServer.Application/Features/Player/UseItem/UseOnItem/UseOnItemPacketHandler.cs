using Mediator;
using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Features.Player.UseItem.UseFieldRune;
using NeoServer.Application.Features.Player.UseItem.UseItem;
using NeoServer.Application.Features.Player.UseItem.UseOnCreature;
using NeoServer.Application.Features.Shared;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.Application.Features.Player.UseItem.UseOnItem;

public class PlayerUseOnItemPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;

    public PlayerUseOnItemPacketHandler(IGameServer game,
        ItemFinder itemFinder, IMediator mediator)
    {
        _game = game;
        _itemFinder = itemFinder;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemOnPacket = new UseItemOnPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        var item = _itemFinder.Find(player, useItemOnPacket.Location, useItemOnPacket.ClientId);
        var (itemTarget, creatureTarget) = GetTarget(player, useItemOnPacket);

        ICommand command = item switch
        {
            IConsumable consumable => new ConsumeItemCommand(player, consumable, creatureTarget),
            IFieldRune fieldRune => new UseFieldRuneCommand(player, fieldRune, useItemOnPacket.ToLocation),
            IUsableOnCreature usableOnCreature =>
                new UseItemOnCreatureCommand(player, usableOnCreature, creatureTarget),
            IUsableOnItem => new UseItemOnItemCommand(player, item, itemTarget),
            IUsableAttackOnCreature attackOnCreature =>
                creatureTarget is null
                    ? new UseItemOnItemCommand(player, item, itemTarget)
                    : new UseItemOnCreatureCommand(player, attackOnCreature, creatureTarget),
            _ => null
        };

        _game.Dispatcher.AddEvent(new Event(2000, () =>
        {
            Guard.ThrowIfAnyNull(command);
            _ = ValueTask.FromResult(_mediator.Send(command));
        }));
    }

    private (IItem Item, ICreature Creature) GetTarget(IPlayer player, UseItemOnPacket useItemPacket)
    {
        switch (useItemPacket.ToLocation.Type)
        {
            case LocationType.Ground when _game.Map[useItemPacket.ToLocation] is { } tile:
                return (tile.TopItemOnStack, tile.TopCreatureOnStack);

            case LocationType.Slot when
                player.Inventory[useItemPacket.ToLocation.Slot] is not null:
                return (player.Inventory[useItemPacket.ToLocation.Slot], null);
        }

        var container = player.Containers[useItemPacket.ToLocation.ContainerId][useItemPacket.ToLocation.ContainerSlot];

        return useItemPacket.ToLocation.Type is LocationType.Container ? (container, null) : (null, null);
    }
}