using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Networking.Packets.Outgoing.Creature;

namespace NeoServer.PacketHandler.Features.Creature.Events;

public class CreatureChangedOutfitEventHandler : IEventHandler
{
    private readonly IGameServer game;
    private readonly IMap map;

    public CreatureChangedOutfitEventHandler(IMap map, IGameServer game)
    {
        this.map = map;
        this.game = game;
    }

    public void Execute(ICreature creature, IOutfit outfit)
    {
        foreach (var spectator in map.GetPlayersAtPositionZone(creature.Location))
        {
            if (!creature.CanSee(spectator.Location)) continue;

            if (!game.CreatureManager.GetPlayerConnection(spectator.CreatureId, out var connection)) continue;
            connection.OutgoingPackets.Enqueue(new CreatureOutfitPacket(creature));
            connection.Send();
        }
    }
}