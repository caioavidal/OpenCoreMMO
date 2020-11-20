using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace NeoServer.Game.World.Map
{
    public class Cylinder : ICylinder
    {
        private readonly IMap _map;
        public ITile FromTile { get; set; }
        public ITile ToTile { get; set; }
        public ICylinderSpectator[] TileSpectators { get; private set; }        

        public Cylinder(IMap map)
        {
            _map = map;
        }

        public Cylinder(IMap map, IThing thing, ITile toTile)
        {
            _map = map;
            ToTile = toTile;

            var spectators = _map.GetCreaturesAtPositionZone(toTile.Location, toTile.Location);

            TileSpectators = new ICylinderSpectator[spectators.Count()];
            int index = 0;
            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator is IPlayer player)
                {
                    toTile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                TileSpectators[index++] = new CylinderSpectator(spectator, 0, stackPosition);
            }
        }

        public void RemoveThing(ref IThing thing, IWalkableTile tile, byte amount = 1)
        {
            FromTile = tile;
            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            TileSpectators = new ICylinderSpectator[spectators.Count()];

            int index = 0;
            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                TileSpectators[index++] = new CylinderSpectator(spectator, stackPosition, 0);
            }

            tile.RemoveThing(ref thing, amount);

        }
        public Result<TileOperationResult> AddThing(ref IThing thing, IWalkableTile tile)
        {
            ToTile = tile;
            var result = tile.AddThing(ref thing);

            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            TileSpectators = new ICylinderSpectator[spectators.Count()];
            int index = 0;

            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                TileSpectators[index++]= new CylinderSpectator(spectator, 0, stackPosition);
            }

            return result;
        }

        public Result<TileOperationResult> MoveThing(ref IMoveableThing thing, IWalkableTile fromTile, IWalkableTile toTile, byte amount = 1)
        {
            ToTile = toTile;
            FromTile = fromTile;

            if (thing is ICreature && toTile.HasCreature)
            {
                return new Result<TileOperationResult>(InvalidOperation.NotPossible);
            }

            var spectators = _map.GetCreaturesAtPositionZone(fromTile.Location, toTile.Location);

            TileSpectators = new ICylinderSpectator[spectators.Count()];
            int index = 0;

            var result = toTile.AddThing(ref thing);

            foreach (var spectator in spectators)
            {
                byte fromStackPosition = default;
                byte toStackPosition = default;

                if (spectator is IPlayer player)
                {
                    fromTile.TryGetStackPositionOfThing(player, thing, out fromStackPosition);
                    toTile.TryGetStackPositionOfThing(player, thing, out toStackPosition);
                }

                TileSpectators[index++] = new CylinderSpectator(spectator, fromStackPosition, toStackPosition);
            }

            var thingToRemove = thing as IThing;
            fromTile.RemoveThing(ref thingToRemove, amount);

            return result;

        }
    }



    public readonly struct CylinderSpectator : IEquatable<CylinderSpectator>, ICylinderSpectator
    {
        public CylinderSpectator(ICreature spectator, byte fromStackPosition, byte toStackPosition)
        {
            FromStackPosition = fromStackPosition;
            ToStackPosition = toStackPosition;
            Spectator = spectator;
        }


        public byte FromStackPosition { get; }
        public byte ToStackPosition { get; }
        public ICreature Spectator { get; }

        public bool Equals([AllowNull] CylinderSpectator other)
        {
            return Spectator.CreatureId == other.Spectator.CreatureId;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Spectator.CreatureId);
        }
    }
}

