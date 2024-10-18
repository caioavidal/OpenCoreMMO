using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing.Creature;

namespace NeoServer.PacketHandler.Features.Creature.Events;

public class CreatureChangedSpeedEventHandler : IEventHandler
{
    private readonly IGameServer game;
    private readonly IMap map;

    public CreatureChangedSpeedEventHandler(IMap map, IGameServer game)
    {
        this.map = map;
        this.game = game;
    }

    public void Execute(IWalkableCreature creature, ushort speed)
    {
        foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
        {
            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;
            connection.OutgoingPackets.Enqueue(new CreatureChangeSpeedPacket(creature.CreatureId, speed));
            connection.Send();
        }
    }
}