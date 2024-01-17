using NeoServer.Application.Common.PacketHandler;
using NeoServer.Game.Common.Contracts.Services;
using NeoServer.Infrastructure.Thread;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Party.InviteToParty;

public class PlayerInviteToPartyPacketHandler : PacketHandler
{
    private readonly IGameServer _game;
    private readonly IPartyInviteService _partyInviteService;

    public PlayerInviteToPartyPacketHandler(IGameServer game, IPartyInviteService partyInviteService)
    {
        _game = game;
        _partyInviteService = partyInviteService;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var creatureId = message.GetUInt32();
        
        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        
        if (!_game.CreatureManager.TryGetPlayer(creatureId, out var invitedPlayer) ||
            !_game.CreatureManager.IsPlayerLogged(invitedPlayer))
        {
            connection.Send(new TextMessagePacket("Invited player is not online.", TextMessageOutgoingType.Small));
            return;
        }

        _game.Dispatcher.AddEvent(new Event(() => _partyInviteService.Invite(player, invitedPlayer)));
    }
}