using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Helpers;
using NeoServer.Networking.Packets.Outgoing.Window;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerReadTextEventHandler : IEventHandler
{
    private readonly IGameServer _game;
    private readonly IItemTextWindowStore _itemTextWindowStore;

    public PlayerReadTextEventHandler(IGameServer game, IItemTextWindowStore itemTextWindowStore)
    {
        _game = game;
        _itemTextWindowStore = itemTextWindowStore;
    }

    public void Execute(IPlayer player, IReadable readable, string text)
    {
        if (Guard.AnyNull(player, readable)) return;

        if (!_game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        var id = _itemTextWindowStore.Add(player, readable);

        connection.OutgoingPackets.Enqueue(new TextWindowPacket(id, readable));

        connection.Send();
    }
}