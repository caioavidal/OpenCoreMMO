using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Application.Infrastructure.Thread;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Party.LeaveParty;

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