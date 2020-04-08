using System.Collections.Generic;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Contracts
{
    public delegate void PlaceCreatureOnMap(ICreature creature);
    public delegate void RemoveThingFromTile(IThing thing, ITile tile, byte fromStackPosition);

    public interface IMap
    {
        ITile this[Location location] { get; }
        ITile this[ushort x, ushort y, sbyte z] { get; }

        event PlaceCreatureOnMap CreatureAddedOnMap;
        event RemoveThingFromTile ThingRemovedFromTile;

        IList<byte> GetDescription(IThing thing, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        void AddCreature(ICreature creature);
        ITile GetNextTile(Location fromLocation, Direction direction);
        IEnumerable<uint> GetCreaturesAtPositionZone(Location location);
        IEnumerable<ITile> GetOffsetTiles(Location location);
        void MoveThing(ref IThing thing, Location toLocation, byte count);
        void RemoveThing(ref IThing thing, ITile tile, byte count);
    }
}
