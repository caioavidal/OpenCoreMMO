using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player.Party;

public class PlayerInviteToPartyHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly IPartyInviteService partyInviteService;

    public PlayerInviteToPartyHandler(IGameServer game, IPartyInviteService partyInviteService)
    {
        this.game = game;
        this.partyInviteService = partyInviteService;
    }

    public override void HandlerMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var creatureId = message.GetUInt32();
        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (!game.CreatureManager.TryGetPlayer(creatureId, out var invitedPlayer) ||
            !game.CreatureManager.IsPlayerLogged(invitedPlayer))
        {
            connection.Send(new TextMessagePacket("Invited player is not online.", TextMessageOutgoingType.Small));
            return;
        }

        game.Dispatcher.AddEvent(new Event(() => partyInviteService.Invite(player, invitedPlayer)));
    }
}