using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player.Party;

public class PlayerLeavePartyHandler : PacketHandler
{
    private readonly IGameServer game;

    public PlayerLeavePartyHandler(IGameServer game)
    {
        this.game = game;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        game.Dispatcher.AddEvent(new Event(() => player.PlayerParty.LeaveParty()));
    }
}