using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NeoServer.Game.World.Map
{
    public class CylinderOperation
    {
        private static IMap _map;
        public static void Setup(IMap map) => _map = map;

        /// <summary>
        /// Creates a cylinder instance as removed
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static Cylinder Removed(IThing thing, byte stackPosition)
        {
            var spectators = _map.GetCreaturesAtPositionZone(thing.Location, thing.Location);

            var tile = _map[thing.Location];
            var tileSpectators = new ICylinderSpectator[spectators.Count()];

            int index = 0;
            foreach (var spectator in spectators)
            {
                if (spectator is IPlayer player)
                {
                    if (thing is IItem item && !item.IsAlwaysOnTop && item is not IGround)
                    {
                        stackPosition = (byte)(tile.GetCreatureStackPositionCount(player) + stackPosition);
                    }
                }

                tileSpectators[index++] = new CylinderSpectator(spectator, stackPosition, stackPosition);
            }
            return new Cylinder(thing, tile, tile, Operation.Removed, tileSpectators);
        }
        public static Cylinder Added(IThing thing)
        {
            var tile = _map[thing.Location];

            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            var tileSpectators = new ICylinderSpectator[spectators.Count()];
            int index = 0;

            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                tileSpectators[index++] = new CylinderSpectator(spectator, stackPosition, stackPosition);
            }

            return new Cylinder(thing, tile, tile, Operation.Added, tileSpectators);
        }
        public static Cylinder Updated(IThing thing, byte amount)
        {
            var tile = _map[thing.Location];

            var spectators = new HashSet<ICylinderSpectator>();
            foreach (var spec in Removed(thing, amount).TileSpectators)
            {
                spectators.Add(spec);
            }
            foreach (var spec in Added(thing).TileSpectators)
            {
                if (spectators.TryGetValue(spec, out var spectator))
                {
                    spectator.ToStackPosition = spec.ToStackPosition;
                }
                else
                {
                    spectators.Add(spec);
                }
            }
            return new Cylinder(thing, tile, tile, Operation.Updated, spectators.ToArray());
        }
        public static Result<OperationResult<IThing>> RemoveThing(IThing thing, IDynamicTile tile, byte amount, out ICylinder cylinder)
        {
            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            var tileSpectators = new ICylinderSpectator[spectators.Count()];

            int index = 0;
            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                tileSpectators[index++] = new CylinderSpectator(spectator, stackPosition, stackPosition);
            }

            var result = tile.RemoveThing(thing, amount, 0, out var removedThing);
            cylinder = new Cylinder(removedThing, tile, tile, Operation.Removed, tileSpectators);
            return result;
        }
        public static Result<OperationResult<IThing>> AddThing(IThing thing, IDynamicTile tile, out ICylinder cylinder)
        {
            var result = tile.AddThing(thing);

            if (result.IsSuccess is false)
            {
                cylinder = null;
                return result;
            }

            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            var tileSpectators = new ICylinderSpectator[spectators.Count()];
            int index = 0;

            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                tileSpectators[index++] = new CylinderSpectator(spectator, stackPosition, stackPosition);
            }

            cylinder = new Cylinder(thing, tile, tile, Operation.Added, tileSpectators);
            return result;
        }

        public static Result<OperationResult<IThing>> MoveThing(IMoveableThing thing, IDynamicTile fromTile, IDynamicTile toTile, byte amount, out ICylinder cylinder)
        {
            amount = amount == 0 ? 1 : amount;

            cylinder = null;

            if (thing is ICreature && toTile.HasCreature) return new Result<OperationResult<IThing>>(InvalidOperation.NotPossible);

            var removeResult = RemoveThing(thing, fromTile, amount, out var removeCylinder);

            if (removeResult.IsSuccess is false) return removeResult;

            var result = AddThing(removeCylinder.Thing, toTile, out var addCylinder);

            if (result.IsSuccess is false)
            {
                cylinder = removeCylinder;
                return result;
            }

            var spectators = new HashSet<ICylinderSpectator>();
            foreach (var spec in removeCylinder.TileSpectators)
            {
                spectators.Add(spec);
            }
            foreach (var spec in addCylinder.TileSpectators)
            {
                if (spectators.TryGetValue(spec, out var spectator))
                {
                    spectator.ToStackPosition = spec.ToStackPosition;
                }
                else
                {
                    spectators.Add(spec);
                }
            }

            foreach (var operation in removeResult.Value.Operations)
            {
                result.Value.Operations?.Add(operation);
            }

            cylinder = new Cylinder(thing, fromTile, toTile, Operation.Moved, spectators.ToArray());
            return result;
        }
    }

    public class CylinderSpectator : IEqualityComparer<ICylinderSpectator>, ICylinderSpectator
    {
        public CylinderSpectator(ICreature spectator, byte fromStackPosition, byte toStackPosition)
        {
            FromStackPosition = fromStackPosition;
            ToStackPosition = toStackPosition;
            Spectator = spectator;
        }

        public byte FromStackPosition { get; set; }
        public byte ToStackPosition { get; set; }
        public ICreature Spectator { get; }
        public override bool Equals(object obj)
        {
            return obj is ICylinderSpectator spec && Spectator == spec.Spectator;
        }

        public bool Equals(ICylinderSpectator x, ICylinderSpectator y)
        {
            return x.Spectator == y.Spectator;
        }

        public override int GetHashCode() => GetHashCode(this);

        public int GetHashCode([DisallowNull] ICylinderSpectator obj) => HashCode.Combine(obj.Spectator.CreatureId);

    }

    public record Cylinder(IThing Thing, ITile FromTile, ITile ToTile, Operation Operation, ICylinderSpectator[] TileSpectators) : ICylinder;
}

