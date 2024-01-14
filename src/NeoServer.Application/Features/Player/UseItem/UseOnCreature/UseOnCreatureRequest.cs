using Mediator;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Features.Player.UseItem.UseFieldRune;
using NeoServer.Application.Features.Player.UseItem.UseItem;
using NeoServer.Application.Features.Shared;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.Items.Types.Usable;
using NeoServer.Infrastructure.Thread;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Player.UseItem.UseOnCreature;

public class PlayerUseItemOnCreatureHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly ItemFinder _itemFinder;
    private readonly IMediator _mediator;

    public PlayerUseItemOnCreatureHandler(IGameServer game, ItemFinder itemFinder, IMediator mediator)
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
            IConsumable consumable => new ConsumeItemCommand(player, consumable, creature),
            IFieldRune fieldRune => new UseFieldRuneCommand(player, fieldRune, creature.Location),
            _ => new UseItemOnCreatureCommand(player, item as IUsableOn, creature)
        };

        _game.Dispatcher.AddEvent(new Event(2000, () => _ = ValueTask.FromResult(_mediator.Send(command))));
    }
}