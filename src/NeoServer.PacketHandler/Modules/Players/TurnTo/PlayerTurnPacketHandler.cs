using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.BuildingBlocks.Infrastructure.Threading.Event;
using NeoServer.Game.Common.Location;
using NeoServer.Networking.Packets.Network;
using NeoServer.Networking.Packets.Network.Enums;

namespace NeoServer.PacketHandler.Modules.Players.TurnTo;

public class PlayerTurnPacketHandler : PacketHandler
{
    private readonly IGameServer _game;

    public PlayerTurnPacketHandler(IGameServer game)
    {
        _game = game;
    }

    public override void HandleMessage(IReadOnlyNetworkMessage message, IConnection connection)
    {
        var direction = ParseTurnPacket(message.IncomingPacket);

        if (!_game.CreatureManager.TryGetPlayer(connection.CreatureId, out var player)) return;

        _game.Dispatcher.AddEvent(new Event(() => player.TurnTo(direction)));
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