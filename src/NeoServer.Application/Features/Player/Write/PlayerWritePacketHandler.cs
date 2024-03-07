using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Networking.Packets.Incoming;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Player.Write;

public class PlayerWritePacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IItemTextWindowStore _itemTextWindowStore;

    public PlayerWritePacketHandler(IGameServer game, IItemTextWindowStore itemTextWindowStore)
    {
        _game = game;
        _itemTextWindowStore = itemTextWindowStore;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var writeTextPacket = new WriteTextPacket(message);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        if (!_itemTextWindowStore.Get(player, writeTextPacket.WindowTextId, out var item)) return;

        if (item is not IReadable readable) return;

        _game.Dispatcher.AddEvent(new Event(() =>
            player.Write(readable, writeTextPacket.Text)));
    }
}