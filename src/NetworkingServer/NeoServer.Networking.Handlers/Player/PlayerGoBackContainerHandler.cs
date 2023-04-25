using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player;

public class PlayerGoBackContainerHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerGoBackContainerHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var containerId = message.GetByte();

        if (_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player))
            _game.Dispatcher.AddEvent(new Event(() =>
                player.Containers.GoBackContainer(containerId))); //todo create a const for 2000 expiration time
    }
}