using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Common.Contracts.Network;
using NeoServer.Server.Common.Contracts.Network.Enums;
using NeoServer.Server.Tasks;

namespace NeoServer.Networking.Handlers.Player.Movement;

public class PlayerTurnHandler : PacketHandler
{
    private readonly IGameServer game;
    private readonly IMap map;

    public PlayerTurnHandler(IGameServer game, IMap map)
    {
        this.game = game;

        this.map = map;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var direction = ParseTurnPacket(message.IncomingPacket);

        if (!game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        game.Dispatcher.AddEvent(new Event(() => player.TurnTo(direction)));
    }

    private Direction ParseTurnPacket(GameIncomingPacketType turnPacket)
    {
        var direction = Direction.North;

        switch (turnPacket)
        {
            case GameIncomingPacketType.TurnNorth:
                direction = Direction.North;
                break;
            case GameIncomingPacketType.TurnEast:
                direction = Direction.East;
                break;
            case GameIncomingPacketType.TurnSouth:
                direction = Direction.South;
                break;
            case GameIncomingPacketType.TurnWest:
                direction = Direction.West;
                break;
        }

        return direction;
    }
}