using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NeoServer.Game.World.Map
{
    public class CylinderOperation
    {
        private static IMap _map;
        public static void Setup(IMap map) => _map = map;

        public static Result<ITileOperationResult> RemoveThing(IThing thing, IDynamicTile tile, byte amount, out ICylinder cylinder)
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

                tileSpectators[index++] = new CylinderSpectator(spectator, stackPosition, 0);
            }

            var result = tile.RemoveThing(thing, amount, out var removedThing);
            cylinder = new Cylinder(removedThing, tile, tile, Operation.Removed, tileSpectators);
            return result;
        }
        public static Result<ITileOperationResult> AddThing(IThing thing, IDynamicTile tile, out ICylinder cylinder)
        {
            var result = tile.AddThing(thing);

            if (result.Success is false)
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

                tileSpectators[index++] = new CylinderSpectator(spectator, 0, stackPosition);
            }

            cylinder = new Cylinder(thing, tile, tile, Operation.Added, tileSpectators);
            return result;
        }

        public static Result<ITileOperationResult> MoveThing(IMoveableThing thing, IDynamicTile fromTile, IDynamicTile toTile, byte amount, out ICylinder cylinder)
        {
            amount = amount == 0 ? 1 : amount;

            cylinder = null;

            if (thing is ICreature && toTile.HasCreature) return new Result<ITileOperationResult>(InvalidOperation.NotPossible);

            var removeResult = RemoveThing(thing, fromTile, amount, out var removeCylinder);

            if (removeResult.Success is false) return removeResult;

            var result = AddThing(removeCylinder.Thing, toTile, out var addCylinder);

            if (result.Success is false)
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

