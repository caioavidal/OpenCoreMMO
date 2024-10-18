using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.PacketHandler.Modules.Party.JoinParty;

public class PlayerJoinPartyPacketHandler(IGameServer game) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var leaderId = message.GetUInt32();

        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (!game.CreatureManager.TryGetPlayer(leaderId, out var leader) ||
            !game.CreatureManager.IsPlayerLogged(leader))
        {
            connection.Send(new TextMessagePacket("Player is not online.", TextMessageOutgoingType.Small));
            return;
        }

        game.Dispatcher.AddEvent(new Event(() => player.PlayerParty.JoinParty(leader.PlayerParty.Party)));
    }
}