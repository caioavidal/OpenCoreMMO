using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts.World
{
    public interface ICylinder
    {
        ICylinderSpectator[] TileSpectators { get; }
        ITile FromTile { get; set; }
        ITile ToTile { get; set; }

        Result<TileOperationResult> AddThing(IThing thing, IDynamicTile tile);
        Result<TileOperationResult> MoveThing(IMoveableThing thing, IDynamicTile fromTile, IDynamicTile toTile, byte amount = 1);
        void RemoveThing(IThing thing, IDynamicTile tile, byte amount = 1);
    }
    public interface ICylinderSpectator
    {
        ICreature Spectator { get; }
        byte FromStackPosition { get; }
        byte ToStackPosition { get; }
    }
}
