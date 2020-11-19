using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
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

        Result<TileOperationResult> AddThing(ref IMoveableThing thing, IWalkableTile tile);
        Result<TileOperationResult> MoveThing(ref IMoveableThing thing, IWalkableTile fromTile, IWalkableTile toTile, byte amount = 1);
        void RemoveThing(ref IMoveableThing thing, IWalkableTile tile, byte amount = 1);
    }
    public interface ICylinderSpectator
    {
        ICreature Spectator { get; }
        byte FromStackPosition { get; }
        byte ToStackPosition { get; }
    }
}
