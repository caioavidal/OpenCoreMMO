using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
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
        event MoveCreatureOnFloor OnCreatureMoved;
        event FailedMoveThing OnThingMovedFailed;
        event AddThingToTile OnThingAddedToTile;
        event UpdateThingOnTile OnThingUpdatedOnTile;

        IList<byte> GetDescription(Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, bool isUnderground, byte windowSizeX = 18, byte windowSizeY = 14);
        bool ArePlayersAround(Location location);
        void AddCreature(ICreature creature);
        ITile GetNextTile(Location fromLocation, Direction direction);
        void RemoveThing(IThing thing, IDynamicTile tile, byte amount = 1);
        IList<byte> GetFloorDescription(Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, byte width, byte height, int verticalOffset, ref int skip);
        IEnumerable<ICreature> GetPlayersAtPositionZone(Location location);
        void AddItem(IThing thing, IDynamicTile tile);
        bool IsInRange(Location start, Location current, Location target, FindPathParams fpp);
        bool CanWalkTo(Location location, out ITile tile);
        HashSet<ICreature> GetCreaturesAtPositionZone(Location location, Location toLocation);
        void PropagateAttack(ICombatActor actor, CombatDamage damage, Coordinate[] area);
        void MoveCreature(IWalkableCreature creature);
        void CreateBloodPool(ILiquid liquid, IDynamicTile tile);
        ITile GetTileDestination(IDynamicTile tile);
        bool TryMoveThing(IMoveableThing thing, Location toLocation, byte amount =1);
        void ReplaceThing(IThing thingToRemove, IThing thingToAdd, byte amount = 1);
        bool CanGoToDirection(Location location, Direction direction, ITileEnterRule rule);
    }
}
