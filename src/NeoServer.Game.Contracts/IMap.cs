using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts
{
    public delegate void PlaceCreatureOnMap(ICreature creature);
    public delegate void RemoveThingFromTile(IThing thing, ITile tile, byte fromStackPosition);
    public delegate void MoveThingOnFloor(IThing thing, ITile fromTile, ITile toTile, byte fromStackPosition);
    public delegate void FailedMoveThing(IThing thing, PathError error);
    public interface IMap
    {
        ITile this[Location location] { get; }
        ITile this[ushort x, ushort y, sbyte z] { get; }

        event PlaceCreatureOnMap OnCreatureAddedOnMap;
        event RemoveThingFromTile OnThingRemovedFromTile;
        event MoveThingOnFloor OnThingMoved;
        event FailedMoveThing OnThingMovedFailed;

        IList<byte> GetDescription(IThing thing, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        void AddCreature(ICreature creature);
        ITile GetNextTile(Location fromLocation, Direction direction);
        IEnumerable<ITile> GetOffsetTiles(Location location);
        bool TryMoveThing(ref IThing thing, Location toLocation, byte count);
        void RemoveThing(ref IThing thing, ITile tile, byte count);
        IList<byte> GetFloorDescription(IThing thing, ushort fromX, ushort fromY, sbyte currentZ, byte width, byte height, int verticalOffset, ref int skip);
        HashSet<uint> GetCreaturesAtPositionZone(Location location, Location toLocation);
        IEnumerable<uint> GetCreaturesAtPositionZone(Location location);
    }
}
