using NeoServer.Application.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Networking.Packets.Outgoing.Player;

namespace NeoServer.Application.Features.Player.Events;

public class PlayerManaChangedEventHandler : IEventHandler
{
    private readonly IGameServer game;

    public PlayerManaChangedEventHandler(IGameServer game)
    {
        this.game = game;
    }

    public void Execute(IPlayer player)
    {
        if (!game.CreatureManager.GetPlayerConnection(player.CreatureId, out var connection)) return;
        connection.OutgoingPackets.Enqueue(new PlayerStatusPacket(player));
        connection.Send();
    }
}