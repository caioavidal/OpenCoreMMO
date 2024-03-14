using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.Application.Features.Party.EnableSharedExperience;

public class PartyEnableSharedExperiencePacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PartyEnableSharedExperiencePacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var experienceSharingActive = message.GetByte() == 1;

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (player == null || player.PlayerParty.IsInParty == false) return;

        player.PlayerParty.Party.IsSharedExperienceEnabled = experienceSharingActive;

        connection.Send(new TextMessagePacket(
            $"Party experience sharing is now {(experienceSharingActive ? "enabled" : "disabled")}.",
            TextMessageOutgoingType.Small));
    }
}