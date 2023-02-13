using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.World;

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