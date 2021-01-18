using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureTeleportedEventHandler : IGameEventHandler
    {
        private readonly IMap map;

        public CreatureTeleportedEventHandler(IMap map)
        {
            this.map = map;
        }

        public void Execute(IWalkableCreature creature, Location location)
        {
            if (map[location] is not IDynamicTile tile || tile.FloorDirection != Common.Location.FloorChangeDirection.None)
            {
                foreach (var neighbour in location.Neighbours)
                {
                    if (map[neighbour] is IDynamicTile toTile && !toTile.HasCreature)
                    {
                        map.TryMoveCreature(creature, toTile.Location);
                        return;
                    }
                }
            }
            else
            {
                map.TryMoveCreature(creature, tile.Location); 
            }
        }
    }
}
