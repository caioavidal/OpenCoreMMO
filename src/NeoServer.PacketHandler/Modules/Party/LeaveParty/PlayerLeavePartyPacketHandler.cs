using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Party.LeaveParty;

public class PlayerLeavePartyPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerLeavePartyPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(() => player.PlayerParty.LeaveParty()));
    }
}