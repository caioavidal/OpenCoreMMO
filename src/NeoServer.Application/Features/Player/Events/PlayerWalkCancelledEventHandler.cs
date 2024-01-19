using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Player;
using NeoServer.Server.Common.Contracts;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerWalkCancelledEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerWalkCancelledEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(ICreature creature)
    {
        if (creature is not IPlayer player) return;

        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;

        connection.OutgoingPackets.Enqueue(new PlayerWalkCancelPacket(player));
        connection.Send();
    }
}