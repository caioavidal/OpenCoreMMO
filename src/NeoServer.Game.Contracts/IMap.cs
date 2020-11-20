using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Item;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts
{
    public delegate void PlaceCreatureOnMap(IWalkableCreature creature, ICylinder cylinder);
    public delegate void RemoveThingFromTile(Items.IThing thing, ICylinder tile);
    public delegate void MoveCreatureOnFloor(IWalkableCreature creature, ICylinder cylinder);
    public delegate void AddThingToTile(Items.IThing thing, ICylinder cylinder);
    public delegate void UpdateThingOnTile(Items.IThing thing, ICylinder cylinder);

    public delegate void FailedMoveThing(Items.IThing thing, InvalidOperation error);
    public interface IMap
    {
        ITile this[Location location] { get; }
        ITile this[ushort x, ushort y, byte z] { get; }

        event PlaceCreatureOnMap OnCreatureAddedOnMap;
        event RemoveThingFromTile OnThingRemovedFromTile;
        event MoveCreatureOnFloor OnThingMoved;
        event FailedMoveThing OnThingMovedFailed;
        event AddThingToTile OnThingAddedToTile;
        event UpdateThingOnTile OnThingUpdatedOnTile;

        IList<byte> GetDescription(Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        bool ArePlayersAround(Location location);
        void AddCreature(ICreature creature);
        ITile GetNextTile(Location fromLocation, Direction direction);
        bool TryMoveThing(ref IMoveableThing thing, Location toLocation);
        void RemoveThing(ref IThing thing, IWalkableTile tile, byte amount = 1);
        IList<byte> GetFloorDescription(Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, byte width, byte height, int verticalOffset, ref int skip);
        IEnumerable<ICreature> GetPlayersAtPositionZone(Location location);
        void AddItem(ref IThing thing, IWalkableTile tile, byte amount = 1);
        bool IsInRange(Location start, Location current, Location target, FindPathParams fpp);
        bool CanWalkTo(Location location, out ITile tile);
        HashSet<ICreature> GetCreaturesAtPositionZone(Location location, Location toLocation);
        void PropagateAttack(ICombatActor actor, CombatDamage damage, Coordinate[] area);
        void MoveCreature(IWalkableCreature creature);
        void CreateBloodPool(ILiquid liquid, IWalkableTile tile);
    }
}
