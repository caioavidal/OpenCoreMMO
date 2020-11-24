using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;

namespace NeoServer.Game.World.Map.Operations
{
    public class MapMoveOperation
    {
        private static IMap Map;

        public static void Init(IMap map) => Map = map;

        //public static Cylinder Move(IMoveableThing thingToMove, IDynamicTile fromTile, IDynamicTile toTile, byte amount = 1)
        //{
        //    Cylinder cylinder = new(Map);

        //    var result = cylinder.MoveThing(thingToMove, fromTile, toTile, amount);
        //    if (result.Success is false)
        //    {
        //        return null;
        //    }
        //    return cylinder;
        //}
    }
}
