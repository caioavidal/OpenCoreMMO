using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Networking.Packets.Network;

namespace NeoServer.PacketHandler.Modules.Session.LogOut;

public class PlayerLogOutPacketHandler(IGameServer gameServer) : PacketHandler
{
    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!gameServer.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        gameServer.Dispatcher.AddEvent(() => player.Logout());
    }
}