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
using System.Text;

namespace NeoServer.Game.World.Map
{
    public class Cylinder : ICylinder
    {
        private readonly IMap _map;
        public ITile FromTile { get; set; }
        public ITile ToTile { get; set; }
        public Dictionary<uint, ICylinderSpectator> TileSpectators { get; private set; }        

        public Cylinder(IMap map)
        {
            _map = map;
        }

        public Cylinder(IMap map, IThing thing, ITile toTile)
        {
            _map = map;
            ToTile = toTile;
            TileSpectators = new Dictionary<uint, ICylinderSpectator>();

            var spectators = _map.GetCreaturesAtPositionZone(toTile.Location, toTile.Location);

            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator.Value is IPlayer player)
                {
                    toTile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                TileSpectators.Add(spectator.Key, new CylinderSpectator(spectator.Value, 0, stackPosition));
            }
        }

        public void RemoveThing(ref IMoveableThing thing, IWalkableTile tile, byte amount = 1)
        {
            FromTile = tile;
            TileSpectators = new Dictionary<uint, ICylinderSpectator>();

            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator.Value is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                TileSpectators.Add(spectator.Key, new CylinderSpectator(spectator.Value, stackPosition, 0));
            }

            tile.RemoveThing(ref thing, amount);

        }
        public Result<TileOperationResult> AddThing(ref IMoveableThing thing, IWalkableTile tile)
        {
            ToTile = tile;
            TileSpectators = new Dictionary<uint, ICylinderSpectator>();

            var result = tile.AddThing(ref thing);

            var spectators = _map.GetCreaturesAtPositionZone(tile.Location, tile.Location);

            foreach (var spectator in spectators)
            {
                byte stackPosition = default;
                if (spectator.Value is IPlayer player)
                {
                    tile.TryGetStackPositionOfThing(player, thing, out stackPosition);
                }

                TileSpectators.Add(spectator.Key, new CylinderSpectator(spectator.Value, 0, stackPosition));
            }

            return result;
        }

        public Result<TileOperationResult> MoveThing(ref IMoveableThing thing, IWalkableTile fromTile, IWalkableTile toTile, byte amount = 1)
        {
            TileSpectators = new Dictionary<uint, ICylinderSpectator>();
            ToTile = toTile;
            FromTile = fromTile;

            if (thing is ICreature && toTile.HasCreature)
            {
                return new Result<TileOperationResult>(InvalidOperation.NotPossible);
            }

            var spectators = _map.GetCreaturesAtPositionZone(fromTile.Location, toTile.Location);

            var result = toTile.AddThing(ref thing);

            foreach (var spectator in spectators)
            {
                byte fromStackPosition = default;
                byte toStackPosition = default;

                if (spectator.Value is IPlayer player)
                {
                    fromTile.TryGetStackPositionOfThing(player, thing, out fromStackPosition);
                    toTile.TryGetStackPositionOfThing(player, thing, out toStackPosition);
                }

                TileSpectators.Add(spectator.Key, new CylinderSpectator(spectator.Value, fromStackPosition, toStackPosition));
            }

            fromTile.RemoveThing(ref thing, amount);

            return result;

        }
    }



    public class CylinderSpectator : ICylinderSpectator
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
    }
}

