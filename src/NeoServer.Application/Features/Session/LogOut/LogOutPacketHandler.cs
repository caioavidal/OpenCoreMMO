using NeoServer.Application.Common.Contracts;
using NeoServer.Application.Common.Contracts.Network;
using NeoServer.Application.Common.PacketHandler;
using NeoServer.Server.Common.Contracts.Network;

namespace NeoServer.Application.Features.Session.LogOut;

public class PlayerLogOutPacketHandler : PacketHandler
{
    private readonly IGameServer _gameServer;

    public PlayerLogOutPacketHandler(IGameServer gameServer)
    {
        _gameServer = gameServer;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        if (!_gameServer.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _gameServer.Dispatcher.AddEvent(() => player.Logout(false));
    }
}