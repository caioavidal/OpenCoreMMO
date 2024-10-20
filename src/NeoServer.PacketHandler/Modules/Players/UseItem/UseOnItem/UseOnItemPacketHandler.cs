using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Domain;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Location;
using NeoServer.Modules.Combat.PlayerAttack;
using NeoServer.Modules.Combat.PlayerAttack.RuneAttack;
using NeoServer.Modules.Players.UseItem.UseFieldRune;
using NeoServer.Modules.Players.UseItem.UseItem;
using NeoServer.Modules.Players.UseItem.UseOnCreature;
using NeoServer.Modules.Players.UseItem.UseOnItem;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Players.UseItem.UseOnItem;

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
            IAttackRune rune => ExecutePlayerRuneAttackCommand(creatureTarget, itemTarget, player, rune),
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

    private static PlayerRuneAttackCommand ExecutePlayerRuneAttackCommand(ICreature creatureTarget, IItem itemTarget,
        IPlayer player, IAttackRune rune)
    {
        var target = (IThing)creatureTarget ?? itemTarget;
        var attackParameters = PlayerAttackParameterBuilder.Build(player, rune, target);
            
        return new PlayerRuneAttackCommand(player, target, rune, attackParameters);
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