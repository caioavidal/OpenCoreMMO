using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.World;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Map.Tiles;

namespace NeoServer.Game.World.Services
{
    public class MapService:IMapService
    {
        private readonly IMap map;

        public MapService(IMap map)
        {
            this.map = map;
        }
        public void ReplaceGround(Location location, IGround ground)
        {
            if (map[location] is not Tile tile) return;
            tile.ReplaceGround(ground);

            if (!tile.HasHole) return;
            
            var finalTile = GetFinalTile(location);

            if (finalTile is not Tile toTile) return;
            
            var removedItems = tile.RemoveAllItems();
            var removedCreatures = tile.RemoveAllCreatures();

            toTile.AddItems(removedItems);
            
            foreach (var removedCreature in removedCreatures)
            {
                map.TryMoveCreature(removedCreature, toTile.Location);
            }
        }

        public void AddItem()
        {
            
        }
        
        public ITile GetFinalTile(Location location)
        {
            var toTile = map[location];
            if (toTile is not IDynamicTile destination) return toTile;
            
            return destination.HasHole ? GetFinalTile(destination.Location.AddFloors(1)) : toTile;
        } 
    }
}