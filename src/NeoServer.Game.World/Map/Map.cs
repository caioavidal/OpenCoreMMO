using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Location;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.World.Map.Tiles;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.World.Map.Operations;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.World.Map
{
    public class Map : IMap
    {

        const int MAP_MAX_LAYERS = 16;

        public event PlaceCreatureOnMap OnCreatureAddedOnMap;
        public event RemoveThingFromTile OnThingRemovedFromTile;
        public event AddThingToTile OnThingAddedToTile;
        public event UpdateThingOnTile OnThingUpdatedOnTile;
        public event MoveCreatureOnFloor OnCreatureMoved;
        public event FailedMoveThing OnThingMovedFailed;
        private readonly World world;

        public Map(World world)
        {
            this.world = world;
            MapReplaceOperation.Init(this);
            MapMoveOperation.Init(this);
            CylinderOperation.Setup(this);
            TileOperationEvent.OnTileChanged += OnTileChanged;
        }

        public void OnTileChanged(ITile tile, IThing thing, OperationResult<IThing> result)
        {
            if (!result.HasAnyOperation) return;
            if (thing is ICreature)
            {
                return;
            }

            foreach (var operation in result.Operations)
            {
                switch (operation.Item2)
                {
                    case Operation.Removed: 
                        if (operation.Item1 is ICumulative cumulative) cumulative.OnReduced -= OnItemReduced;
                        OnThingRemovedFromTile?.Invoke(operation.Item1, CylinderOperation.Removed(operation.Item1, operation.Item3));
                        break;
                    case Operation.Updated:
                        OnThingUpdatedOnTile?.Invoke(operation.Item1, CylinderOperation.Updated(operation.Item1, operation.Item1.Amount));
                        break;
                    case Operation.Added: 
                        if(operation.Item1 is ICumulative c) c.OnReduced += OnItemReduced;
                        OnThingAddedToTile?.Invoke(operation.Item1, CylinderOperation.Added(operation.Item1));
                        break;
                    default: break;
                }
            }
        }

        public void ReplaceThing(IThing thingToRemove, IThing thingToAdd, byte amount = 1)
        {
            var result = MapReplaceOperation.Replace(thingToRemove, thingToAdd, amount);
            OnThingRemovedFromTile?.Invoke(thingToRemove, result.Item1);
            OnThingAddedToTile?.Invoke(thingToAdd, result.Item2);

            if (thingToAdd is IGround ground && this[thingToAdd.Location] is IDynamicTile tile && tile.HasCreature)
            {
                if (GetTileDestination(tile) is not IDynamicTile destination) return;

                foreach (var creature in tile.Creatures.Values)
                {
                    TryMoveThing(creature, destination.Location);
                }
            }
        }

        public ITile this[Location location] => world.TryGetTile(ref location, out ITile tile) ? tile : null;
        public ITile this[ushort x, ushort y, byte z] => this[new Location(x, y, z)];

        public bool TryMoveThing(IMoveableThing thing, Location toLocation, byte amount = 1)
        {
            if (this[thing.Location] is not IDynamicTile fromTile)
            {
                OnThingMovedFailed(thing, InvalidOperation.NotPossible);
                return false;
            }

            if (this[toLocation] is not IDynamicTile toTile)//immutable tiles cannot be modified
            {
                OnThingMovedFailed(thing, InvalidOperation.NotEnoughRoom);
                return false;
            }

            if (thing is IWalkableCreature creature)
            {
                SwapCreatureBetweenSectors(creature, toLocation);

                var result = CylinderOperation.MoveThing(thing, fromTile, toTile, amount, out ICylinder cylinder);
                if (result.IsSuccess is false)
                {
                    RollbackSwapCreatureBetweenSectors(creature, toLocation);

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
                OnCreatureMoved?.Invoke(creature, cylinder);
            }

            var tileDestination = GetTileDestination(toTile);

            if (tileDestination == toTile)
            {
                return true;
            }

            return TryMoveThing(thing, tileDestination.Location);
        }

        private void SwapCreatureBetweenSectors(ICreature creature, Location toLocation)
        {
            var oldSector = world.GetSector(creature.Location.X, creature.Location.Y);
            var newSector = world.GetSector(toLocation.X, toLocation.Y);

            if (oldSector != newSector)
            {
                oldSector.RemoveCreature(creature);
                newSector.AddCreature(creature);
            }
        }
        private void RollbackSwapCreatureBetweenSectors(ICreature creature, Location toLocation)
        {
            var oldSector = world.GetSector(creature.Location.X, creature.Location.Y);
            var newSector = world.GetSector(toLocation.X, toLocation.Y);

            if (oldSector != newSector)
            {
                newSector.RemoveCreature(creature);
                oldSector.AddCreature(creature);
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

        public ITile GetTileDestination(IDynamicTile tile)
        {
            Func<ITile, FloorChangeDirection, bool> hasFloorDestination = (tile, direction) => tile is IDynamicTile walkable ? walkable.FloorDirection == direction : false;

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
        public void RemoveThing(IThing thing, IDynamicTile tile, byte amount = 1)
        {
            if (tile.RemoveThing(thing, amount, 0, out var removedThing).IsSuccess is false) return;
            if (thing is IMoveableThing moveable) moveable.OnMoved();
        }
        public void OnItemReduced(ICumulative item, byte amount)
        {
            if (item.Amount == 0 && this[item.Location] is IDynamicTile tile) 
                RemoveThing(item, tile, amount);
            if (item.Amount > 0) 
                OnThingUpdatedOnTile?.Invoke(item, CylinderOperation.Removed(item, amount));
        }
        public void AddItem(IThing thing, IDynamicTile tile)
        {
            var result = CylinderOperation.AddThing(thing, tile, out ICylinder cylinder);
            if (result.IsSuccess is false) return;

            if (thing is IMoveableThing moveable) moveable.OnMoved();
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
                        else if (tile is IDynamicTile mutableTile)
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
            var toLocation = fromLocation.GetNextLocation(direction);
            return this[toLocation];
        }
        public void AddCreature(ICreature creature)
        {
            var thing = creature as IThing;

            if (this[creature.Location] is IDynamicTile tile)
            {
                var sector = world.GetSector(creature.Location.X, creature.Location.Y);
                sector.AddCreature(creature);

                if (CylinderOperation.AddThing(thing, tile, out ICylinder cylinder).IsSuccess is false) return;

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
                if (tile is IDynamicTile walkableTile)
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
            if (creature.TryGetNextStep(out var direction))
            {
                var fromTile = creature.Tile;

                if (GetNextTile(creature.Location, direction) is not IDynamicTile toTile || !TryMoveThing(creature, toTile.Location))
                {
                    if (creature is IPlayer player) player.CancelWalk();
                    return;
                }

                creature.OnMoved(fromTile, toTile);
            }

            if (creature.IsRemoved)
            {
                creature.StopWalking();
                return;
            }
        }
        public void CreateBloodPool(ILiquid pool, IDynamicTile tile)
        {
            if (tile?.TopItems != null && tile.TopItems.TryPeek(out var topItem) && topItem is ILiquid && topItem is IThing topItemThing)
            {
                RemoveThing(topItemThing, tile, 1);
            }

            if (pool is null) return;

            var poolThing = pool as IThing;
            AddItem(poolThing, tile);
        }
        public bool CanGoToDirection(Location location, Direction direction, ITileEnterRule rule)
        {
            var tile = GetNextTile(location, direction);
            return rule.CanEnter(tile);
        }
    }

}


