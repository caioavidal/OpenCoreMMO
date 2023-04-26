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
}

public interface ICylinderSpectator
{
    ICreature Spectator { get; }
    byte FromStackPosition { get; set; }
    byte ToStackPosition { get; set; }
}