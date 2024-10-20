using Mediator;
using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Domain;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Modules.Combat.PlayerAttack;
using NeoServer.Modules.Combat.PlayerAttack.RuneAttack;
using NeoServer.Modules.Players.UseItem.UseFieldRune;
using NeoServer.Modules.Players.UseItem.UseItem;
using NeoServer.Modules.Players.UseItem.UseOnCreature;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Players.UseItem.UseOnCreature;

public class PlayerUseItemOnCreaturePacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;

    public PlayerUseItemOnCreaturePacketHandler(IGameServer game, ItemFinder itemFinder, IMediator mediator)
    {
        _game = game;
        _itemFinder = itemFinder;
        _mediator = mediator;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var useItemOnPacket = new UseItemOnCreaturePacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (!_game.CreatureManager.TryGetCreature(useItemOnPacket.CreatureId, out var creature)) return;

        var item = _itemFinder.Find(player, useItemOnPacket.FromLocation, useItemOnPacket.ClientId);

        ICommand command = item switch
        {
            IAttackRune rune => new PlayerRuneAttackCommand(player, creature,rune, PlayerAttackParameterBuilder.Build(player, rune)),
            IConsumable consumable => new ConsumeItemCommand(player, consumable, creature),
            IFieldRune fieldRune => new UseFieldRuneCommand(player, fieldRune, creature.Location),
            _ => new UseItemOnCreatureCommand(player, item as IUsableOn, creature)
        };

        _game.Dispatcher.AddEvent(new Event(2000, () => _ = ValueTask.FromResult(_mediator.Send(command))));
    }
}