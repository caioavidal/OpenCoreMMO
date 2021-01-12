using NeoServer.Game.Common;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;

namespace NeoServer.Game.Contracts.World
{
    public interface ICylinder
    {
        ICylinderSpectator[] TileSpectators { get; }
        ITile FromTile { get; }
        ITile ToTile { get; }
        IThing Thing { get; }
        Operation Operation { get; }
        bool IsTeleport { get; }
        //Result<TileOperationResult> AddThing(IThing thing, IDynamicTile tile);
        //Result<TileOperationResult> MoveThing(IMoveableThing thing, IDynamicTile fromTile, IDynamicTile toTile, byte amount = 1);
        //IThing RemoveThing(IThing thing, IDynamicTile tile, byte amount = 1);
    }
    public interface ICylinderSpectator
    {
        ICreature Spectator { get; }
        byte FromStackPosition { get; set; }
        byte ToStackPosition { get; set; }
    }
}
