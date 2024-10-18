using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Outgoing;

namespace NeoServer.PacketHandler.Modules.Party.EnableSharedExperience;

public class PartyEnableSharedExperiencePacketHandler(IGameServer game) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var experienceSharingActive = message.GetByte() == 1;

        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;
        if (player == null || player.PlayerParty.IsInParty == false) return;

        player.PlayerParty.Party.IsSharedExperienceEnabled = experienceSharingActive;

        connection.Send(new TextMessagePacket(
            $"Party experience sharing is now {(experienceSharingActive ? "enabled" : "disabled")}.",
            TextMessageOutgoingType.Small));
    }
}