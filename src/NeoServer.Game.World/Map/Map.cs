using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Combat.Structs;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoServer.Game.World.Map
{

    public class Map : IMap
    {

        // Start positions
        public static Location NewbieStart = new Location { X = 1000, Y = 1000, Z = 7 };
        public static Location VeteranStart = new Location { X = 1000, Y = 1000, Z = 7 };
        const int MAP_MAX_LAYERS = 16;

        public event PlaceCreatureOnMap OnCreatureAddedOnMap;
        public event RemoveThingFromTile OnThingRemovedFromTile;
        public event AddThingToTile OnThingAddedToTile;
        public event UpdateThingOnTile OnThingUpdatedOnTile;
        public event MoveCreatureOnFloor OnThingMoved;
        public event FailedMoveThing OnThingMovedFailed;

        private readonly World world;

        public Map(World world)
        {
            this.world = world;
        }

        public ITile this[Location location]
        {
            get
            {
                if (world.TryGetTile(ref location, out ITile tile))
                {
                    if (tile is IWalkableTile walkableTile)
                    {
                        //  walkableTile.OnThingAddedToTile -= AttachEvent;
                        //  walkableTile.OnThingAddedToTile += AttachEvent;
                    }
                    return tile;
                }
                return null;
            }
        }

        public void AttachEvent(IThing thing, ITile tile) => OnThingAddedToTile?.Invoke(thing, new Cylinder(this, thing, tile));

        public ITile this[ushort x, ushort y, byte z] => this[new Location(x, y, z)];

        public bool TryMoveThing(ref IMoveableThing thing, Location toLocation)
        {
            if (this[thing.Location] is IStaticTile)
            {
                OnThingMovedFailed(thing, InvalidOperation.NotPossible);
                return false;
            }

            if (this[toLocation] is IStaticTile)//immutable tiles cannot be modified
            {
                OnThingMovedFailed(thing, InvalidOperation.NotEnoughRoom);
                return false;
            }

            var fromTile = this[thing.Location] as IWalkableTile;
            var toTile = this[toLocation] as IWalkableTile;

            SwapCreatureBetweenSectors(thing, toLocation);

            var cylinder = new Cylinder(this);
            //todo: not thread safe
            var result = cylinder.MoveThing(ref thing, fromTile, toTile);
            if (!result.Success)
            {
                return false;
            }

            foreach (var spectator in cylinder.TileSpectators)
            {
                if (spectator.Spectator is IMonster monsterSpectator && thing is IPlayer playerWalking)
                {
                    monsterSpectator.SetAsEnemy(playerWalking);
                }
                if (spectator.Spectator is IPlayer playerSpectator && thing is IMonster monsterWalking)
                {
                    monsterWalking.SetAsEnemy(playerSpectator);
                }
            }


        

            OnThingMoved?.Invoke((IWalkableCreature)thing, cylinder);

            var tileDestination = GetTileDestination(toTile);

            if (tileDestination == toTile)
            {
                return true;
            }

            return TryMoveThing(ref thing, tileDestination.Location);
        }

        private void SwapCreatureBetweenSectors(IMoveableThing thing, Location toLocation)
        {
            if (thing is ICreature creature)
            {
                var oldSector = world.GetSector(thing.Location.X, thing.Location.Y);
                var newSector = world.GetSector(toLocation.X, toLocation.Y);

                if (oldSector != newSector)
                {
                    oldSector.RemoveCreature(creature);
                    newSector.AddCreature(creature);
                }
            }
        }

        public bool IsInRange(Location start, Location current, Location target, FindPathParams fpp)
        {
            if (fpp.FullPathSearch)
            {
                if (current.X > target.X + fpp.MaxTargetDist)
                {
                    return false;
                }

                if (current.X < target.X - fpp.MaxTargetDist)
                {
                    return false;
                }

                if (current.Y > target.Y + fpp.MaxTargetDist)
                {
                    return false;
                }

                if (current.Y < target.Y - fpp.MaxTargetDist)
                {
                    return false;
                }
            }
            else
            {
                int dx = start.GetSqmDistanceX(target);

                int dxMax = (dx >= 0 ? fpp.MaxTargetDist : 0);
                if (current.X > target.X + dxMax)
                {
                    return false;
                }

                int dxMin = (dx <= 0 ? fpp.MaxTargetDist : 0);
                if (current.X < target.X - dxMin)
                {
                    return false;
                }

                int dy = start.GetSqmDistanceY(target);

                int dyMax = (dy >= 0 ? fpp.MaxTargetDist : 0);
                if (current.Y > target.Y + dyMax)
                {
                    return false;
                }

                int dyMin = (dy <= 0 ? fpp.MaxTargetDist : 0);
                if (current.Y < target.Y - dyMin)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanWalkTo(Location location, out ITile tile)
        {

            tile = this[location];

            if (tile == null || tile is IStaticTile)
            {
                return false;
            }

            if (!(tile is IWalkableTile walkableTile))
            {
                return false;
            }

            if (walkableTile.HasCreature) return false;
            if (walkableTile.HasBlockPathFinding) return false;

            return true;
        }

        private ITile GetTileDestination(IWalkableTile tile)
        {
            Func<ITile, FloorChangeDirection, bool> hasFloorDestination = (tile, direction) => tile is IWalkableTile walkable ? walkable.FloorDirection == direction : false;

            var x = tile.Location.X;
            var y = tile.Location.Y;
            var z = tile.Location.Z;

            if (hasFloorDestination(tile, FloorChangeDirection.Down))
            {
                z++;

                var southDownTile = this[x, (ushort)(y - 1), z];

                if (hasFloorDestination(southDownTile, FloorChangeDirection.SouthAlternative))
                {
                    y -= 2;
                    return this[x, y, z] ?? tile;
                }

                var eastDownTile = this[(ushort)(x - 1), y, z];

                if (hasFloorDestination(eastDownTile, FloorChangeDirection.EastAlternative))
                {
                    x -= 2;
                    return this[x, y, z] ?? tile;
                }

                var downTile = this[x, y, z];

                if (downTile == null)
                {
                    return tile;
                }

                if (hasFloorDestination(downTile, FloorChangeDirection.North))
                {
                    ++y;
                }
                if (hasFloorDestination(downTile, FloorChangeDirection.South))
                {
                    --y;
                }
                if (hasFloorDestination(downTile, FloorChangeDirection.SouthAlternative))
                {
                    y -= 2;
                }
                if (hasFloorDestination(downTile, FloorChangeDirection.East))
                {
                    --x;
                }
                if (hasFloorDestination(downTile, FloorChangeDirection.EastAlternative))
                {
                    x -= 2;
                }
                if (hasFloorDestination(downTile, FloorChangeDirection.West))
                {
                    ++x;
                }

                return this[x, y, z] ?? tile;
            }
            if (tile.FloorDirection != default) //has any floor destination check
            {
                z--;

                if (hasFloorDestination(tile, FloorChangeDirection.North))
                {
                    --y;
                }
                if (hasFloorDestination(tile, FloorChangeDirection.South))
                {
                    ++y;
                }
                if (hasFloorDestination(tile, FloorChangeDirection.SouthAlternative))
                {
                    y += 2;
                }
                if (hasFloorDestination(tile, FloorChangeDirection.East))
                {
                    ++x;
                }
                if (hasFloorDestination(tile, FloorChangeDirection.EastAlternative))
                {
                    x += 2;
                }
                if (hasFloorDestination(tile, FloorChangeDirection.West))
                {
                    --x;
                }

                return this[x, y, z] ?? tile;

            }

            return tile;
        }
        public void RemoveThing(ref IMoveableThing thing, IWalkableTile tile, byte amount = 1)
        {
            Cylinder cylinder = new Cylinder(this);

            cylinder.RemoveThing(ref thing, tile, amount);

            OnThingRemovedFromTile?.Invoke(thing, cylinder);
        }
        public void AddItem(ref IMoveableThing thing, IWalkableTile tile, byte amount = 1)
        {

            var cylinder = new Cylinder(this);
            var result = cylinder.AddThing(ref thing, tile);

            foreach (var operation in result.Value.Operations)
            {
                switch (operation)
                {
                    case Operation.Added: OnThingAddedToTile?.Invoke(thing, cylinder); break;
                    case Operation.Updated: OnThingUpdatedOnTile?.Invoke(tile.TopItemOnStack, cylinder); break;
                    default: break;
                }
            }
        }

        public IEnumerable<ICreature> GetPlayersAtPositionZone(Location location)
        {
            return GetCreaturesAtPositionZone(location, onlyPlayers: true);
        }
        public HashSet<ICreature> GetCreaturesAtPositionZone(Location location, Location toLocation)
        {
            if (location == toLocation)
            {
                return GetCreaturesAtPositionZone(location).ToHashSet();
            }

            var fromSpectators = GetCreaturesAtPositionZone(location);
            var toSpectators = GetCreaturesAtPositionZone(toLocation);

            var spectators = new List<ICreature>(fromSpectators.Count() + toSpectators.Count());

            spectators.AddRange(fromSpectators);
            spectators.AddRange(toSpectators);
            return spectators.ToHashSet();

        }

        public IEnumerable<ICreature> GetCreaturesAtPositionZone(Location location, bool onlyPlayers = false)
        {

            if (location.Z > MAP_MAX_LAYERS) return new List<ICreature>(0);

            var viewPortX = (ushort)MapViewPort.ViewPortX;
            var viewPortY = (ushort)MapViewPort.ViewPortY;

            int minZ = 0;
            int maxZ;
            if (location.IsUnderground)
            {
                minZ = Math.Max(location.Z - 2, 0);
                maxZ = Math.Min(location.Z + 2, 15); //15 = max floor value
            }
            else if (location.Z == 6)
            {
                maxZ = 8;
            }
            else if (location.IsSurface)
            {
                maxZ = 9;
            }
            else
            {
                maxZ = 7;
            }

            //if (location.Z != toLocation.Z) //if player changed floor, we have to increase the min and max z range
            //{
            //    minZ = Math.Max(minZ - 1, 0);
            //    maxZ = Math.Max(maxZ + 1, 15);
            //}

            var minX = (ushort)(location.X + -viewPortX);
            var minY = (ushort)(location.Y + -viewPortY);
            var maxX = (ushort)(location.X + viewPortX);
            var maxY = (ushort)(location.Y + viewPortY);

            var search = new SpectatorSearch(ref location, minRangeX: -viewPortX, minRangeY: -viewPortY, maxRangeX: viewPortX, maxRangeY: viewPortY, minRangeZ: minZ, maxRangeZ: maxZ, onlyPlayers: onlyPlayers);
            return world.GetSpectators(ref search);
        }


        public IList<byte> GetDescription(Contracts.Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, bool isUnderground, byte windowSizeX = MapConstants.DefaultMapWindowSizeX, byte windowSizeY = MapConstants.DefaultMapWindowSizeY)
        {
            var tempBytes = new List<byte>();

            var skip = -1;

            // we crawl from the ground up to the very top of the world (7 -> 0).
            int crawlTo;
            int crawlFrom;
            int crawlDelta;
            // Unless... we're undeground.
            // Then we crawl from 2 floors up, this, and 2 floors down for a total of 5 floors.
            if (currentZ > 7)//isUnderground
            {
                crawlDelta = 1;
                crawlFrom = currentZ - 2;
                crawlTo = Math.Min(15, currentZ + 2);
            }
            else
            {
                crawlFrom = 7;
                crawlTo = 0;
                crawlDelta = -1;
            }

            for (var nz = crawlFrom; nz != crawlTo + crawlDelta; nz += crawlDelta)
            {
                tempBytes.AddRange(GetFloorDescription(thing, fromX, fromY, (byte)nz, windowSizeX, windowSizeY, currentZ - nz, ref skip));
            }

            if (skip >= 0)
            {
                tempBytes.Add((byte)skip);
                tempBytes.Add(0xFF);
            }

            return tempBytes;
        }

        public IList<byte> GetFloorDescription(Contracts.Items.IThing thing, ushort fromX, ushort fromY, byte currentZ, byte width, byte height, int verticalOffset, ref int skip)
        {
            var tempBytes = new List<byte>();

            byte start = 0xFE;
            byte end = 0xFF;

            for (var nx = 0; nx < width; nx++)
            {
                for (var ny = 0; ny < height; ny++)
                {

                    var tile = this[(ushort)(fromX + nx + verticalOffset), (ushort)(fromY + ny + verticalOffset), currentZ];

                    if (tile != null)
                    {
                        if (skip >= 0)
                        {
                            tempBytes.Add((byte)skip);
                            tempBytes.Add(end);
                        }

                        skip = 0;

                        if (tile is IStaticTile immutableTile)
                        {
                            tempBytes.AddRange(immutableTile.Raw);
                        }
                        else if (tile is IWalkableTile mutableTile)
                        {
                            tempBytes.AddRange(mutableTile.GetRaw(thing as IPlayer));
                        }

                    }
                    else if (skip == start)
                    {
                        tempBytes.Add(end);
                        tempBytes.Add(end);
                        skip = -1;
                    }
                    else
                    {
                        ++skip;
                    }
                }
            }

            return tempBytes;
        }

        public ITile GetNextTile(Location fromLocation, Direction direction)
        {
            var toLocation = fromLocation;

            switch (direction)
            {
                case Direction.East:
                    toLocation.X += 1;
                    break;
                case Direction.West:
                    toLocation.X -= 1;
                    break;
                case Direction.North:
                    toLocation.Y -= 1;
                    break;
                case Direction.South:
                    toLocation.Y += 1;
                    break;
                case Direction.NorthEast:
                    toLocation.X += 1;
                    toLocation.Y -= 1;
                    break;
                case Direction.NorthWest:
                    toLocation.X -= 1;
                    toLocation.Y -= 1;
                    break;
                case Direction.SouthEast:
                    toLocation.X += 1;
                    toLocation.Y += 1;
                    break;
                case Direction.SouthWest:
                    toLocation.X -= 1;
                    toLocation.Y += 1;
                    break;

            }

            return this[toLocation];
        }

        public void AddCreature(ICreature creature)
        {
            var thing = creature as IMoveableThing;

            if (this[creature.Location] is IWalkableTile tile)
            {
                var sector = world.GetSector(creature.Location.X, creature.Location.Y);
                sector.AddCreature(creature);

                var cylinder = new Cylinder(this);
                cylinder.AddThing(ref thing, tile);

                if (creature is IPlayer player)
                {
                    foreach (var spectator in cylinder.TileSpectators)
                    {
                        if (spectator.Spectator is IMonster monster)
                        {
                            monster.SetAsEnemy(player);
                        }
                    }
                }

                if (creature is IWalkableCreature walkableCreature) OnCreatureAddedOnMap?.Invoke(walkableCreature, cylinder);
            }
        }

        public bool ArePlayersAround(Location location) => GetPlayersAtPositionZone(location).Any();

        public void PropagateAttack(ICombatActor actor, CombatDamage damage, Coordinate[] area)
        {
            foreach (var coordinates in area)
            {
                var tile = this[coordinates.Location];
                if (tile is IWalkableTile walkableTile)
                {
                    foreach (var target in walkableTile.Creatures.Values)
                    {
                        if (actor == target) continue;
                        if (!(target is ICombatActor targetCreature)) continue;
                        
                        targetCreature.ReceiveAttack(actor, damage);
                    }
                }
            }
        }

        public void MoveCreature(IWalkableCreature creature)
        {
            var thing = creature as IMoveableThing;

            if (creature.TryGetNextStep(out var direction))
            {
                var toTile = GetNextTile(thing.Location, direction);

                if (!TryMoveThing(ref thing, toTile.Location))
                {
                    if (creature is IPlayer player) player.CancelWalk();
                }
            }

            if (creature.IsRemoved)
            {
                creature.StopWalking();
                return;
            }
        }
    }
}
