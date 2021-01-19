using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts
{
    public delegate void PlaceCreatureOnMap(IWalkableCreature creature, ICylinder cylinder);
    public delegate void RemoveThingFromTile(Items.IThing thing, ICylinder cylinder);
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
        event MoveCreatureOnFloor OnCreatureMoved;
        event FailedMoveThing OnThingMovedFailed;
        event AddThingToTile OnThingAddedToTile;
        event UpdateThingOnTile OnThingUpdatedOnTile;

        IList<byte> GetDescription(Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        bool ArePlayersAround(Location location);
        void PlaceCreature(ICreature creature);
        ITile GetNextTile(Location fromLocation, Direction direction);
        IList<byte> GetFloorDescription(Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, byte width, byte height, int verticalOffset, ref int skip);
        IEnumerable<ICreature> GetPlayersAtPositionZone(Location location);
        bool IsInRange(Location start, Location current, Location target, FindPathParams fpp);
        HashSet<ICreature> GetCreaturesAtPositionZone(Location location, Location toLocation);
        void PropagateAttack(ICombatActor actor, CombatDamage damage, Coordinate[] area);
        void MoveCreature(IWalkableCreature creature);
        void CreateBloodPool(ILiquid liquid, IDynamicTile tile);
        ITile GetTileDestination(ITile tile);
        bool TryMoveCreature(ICreature creature, Location toLocation);
        bool CanGoToDirection(Location location, Direction direction, ITileEnterRule rule);
        void RemoveCreature(ICreature creature);
        void SwapCreatureBetweenSectors(ICreature creature, Location fromLocation, Location toLocation);
        HashSet<ICreature> GetSpectators(Location fromLocation, Location toLocation, bool onlyPlayers = false);
        HashSet<ICreature> GetSpectators(Location fromLocation, bool onlyPlayers = false);
        IEnumerable<ICreature> GetCreaturesAtPositionZone(Location location, bool onlyPlayers = false);
    }
}
