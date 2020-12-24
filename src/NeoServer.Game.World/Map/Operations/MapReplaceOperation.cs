using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.World.Map.Operations
{
    public class MapReplaceOperation
    {
        private static IMap Map;

        public static void Init(IMap map) => Map = map;

        public static (ICylinder, ICylinder) Replace(IThing thingToRemove, IThing thingToCreate, byte amount = 1)
        {
            if (Map[thingToRemove.Location] is not IDynamicTile tile) return new(null, null);

            var result = CylinderOperation.RemoveThing(thingToRemove, tile, amount, out var removeCylinder);
            CylinderOperation.AddThing(thingToCreate, tile, out ICylinder cylinder);

            return new(removeCylinder, cylinder);
        }
    }
}
