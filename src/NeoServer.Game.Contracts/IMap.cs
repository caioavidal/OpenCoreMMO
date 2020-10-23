using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts
{
    public delegate void PlaceCreatureOnMap(ICreature creature, ICylinder cylinder);
    public delegate void RemoveThingFromTile(Items.IThing thing, ICylinder tile);
    public delegate void MoveCreatureOnFloor(ICreature creature, ICylinder cylinder);
    public delegate void AddThingToTile(Items.IThing thing, ICylinder cylinder);
    public delegate void UpdateThingOnTile(Items.IThing thing, ICylinder cylinder);

    public delegate void FailedMoveThing(Items.IThing thing, InvalidOperation error);
    public interface IMap
    {
        ITile this[Location location] { get; }
        ITile this[ushort x, ushort y, sbyte z] { get; }

        event PlaceCreatureOnMap OnCreatureAddedOnMap;
        event RemoveThingFromTile OnThingRemovedFromTile;
        event MoveCreatureOnFloor OnThingMoved;
        event FailedMoveThing OnThingMovedFailed;
        event AddThingToTile OnThingAddedToTile;
        event UpdateThingOnTile OnThingUpdatedOnTile;

        IList<byte> GetDescription(Items.IThing thing, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        bool ArePlayersAround(Location location);
        void AddCreature(ICreature creature);
        ITile GetNextTile(Location fromLocation, Direction direction);
        IEnumerable<ITile> GetOffsetTiles(Location location);
        bool TryMoveThing(ref IMoveableThing thing, Location toLocation);
        void RemoveThing(ref IMoveableThing thing, IWalkableTile tile, byte amount = 1);
        IList<byte> GetFloorDescription(Items.IThing thing, ushort fromX, ushort fromY, sbyte currentZ, byte width, byte height, int verticalOffset, ref int skip);
        IEnumerable<uint> GetPlayersAtPositionZone(Location location);
        void AddItem(ref IMoveableThing thing, IWalkableTile tile, byte amount = 1);
        bool IsInRange(Location start, Location current, Location target, FindPathParams fpp);
        bool CanWalkTo(Location location, out ITile tile);
        Dictionary<uint, ICreature> GetCreaturesAtPositionZone(Location location, Location toLocation);
        void PropagateAttack(ICreature creature, ICreature victim, ICombatAttack combatAttack);
    }
}
