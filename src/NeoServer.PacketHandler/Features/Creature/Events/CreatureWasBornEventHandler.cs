using NeoServer.BuildingBlocks.Application.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.PacketHandler.Features.Creature.Events;

public class CreatureWasBornEventHandler : IEventHandler
{
    private readonly IMap map;

    public CreatureWasBornEventHandler(IMap map)
    {
        this.map = map;
    }

    public void Execute(IMonster creature, Location location)
    {
        map.PlaceCreature(creature);
    }
}