using BenchmarkDotNet.Attributes;
using NeoServer.Game.Contracts;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.World.Map;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoServer.Benchmarks.World
{

    public class Tile
    {
        public List<uint> CreatureIds { get; set; } = new List<uint>();
        private readonly Coordinate coordinate;
        public Tile(Coordinate coordinate)
        {
            this.coordinate = coordinate;
        }
    }
    public class GetCreaturesAtPositionZoneBenchmark
    {
        private Dictionary<Coordinate, Tile> tiles;
        public GetCreaturesAtPositionZoneBenchmark()
        {
            tiles = GenerateTiles();
        }

        public Tile this[int x, int y, sbyte z]
        {
            get
            {
                if (tiles.TryGetValue(new Coordinate(x, y, z), out Tile tile))
                {
                    return tile;
                }
                return null;
            }
        }
        private Dictionary<Coordinate, Tile> GenerateTiles()
        {
            var tiles = new Dictionary<Coordinate, Tile>();

            for (int x = 1000; x < 2000; x++)
            {
                for (int y = 1000; y < 2000; y++)
                {
                    var coord = new Coordinate(x, y, 7);
                    var tile = new Tile(coord);

                    for (uint i = 0; i < 10; i++)
                    {
                        tile.CreatureIds.Add(i);
                    }

                    tiles.Add(coord, tile);
                }
            }
            return tiles;
        }

        [Benchmark]
        public List<uint> GetCreaturesAtPositionZone()
        {
            var result = new List<uint>();

            var location = new Location() { X = 1050, Y = 1090, Z = 7 };

            var minX = (ushort)(location.X + -(ushort)MapViewPort.ViewPortX);
            var minY = (ushort)(location.Y + -(ushort)MapViewPort.ViewPortY);
            var maxX = (ushort)(location.X + MapViewPort.ViewPortX);
            var maxY = (ushort)(location.Y + MapViewPort.ViewPortY);

            for (ushort x = minX; x <= maxX; x++)
            {
                for (ushort y = minY; y <= maxY; y++)
                {
                    Tile tile = this[x, y, location.Z];
                    if (tile != null)
                    {
                        foreach (var creature in tile.CreatureIds)
                        {
                            result.Add(creature);
                        }
                    }
                }
            }

            location = new Location() { X = 1051, Y = 1090, Z = 7 };

            minX = (ushort)(location.X + -(ushort)MapViewPort.ViewPortX);
            minY = (ushort)(location.Y + -(ushort)MapViewPort.ViewPortY);
            maxX = (ushort)(location.X + MapViewPort.ViewPortX);
            maxY = (ushort)(location.Y + MapViewPort.ViewPortY);

            for (ushort x = minX; x <= maxX; x++)
            {
                for (ushort y = minY; y <= maxY; y++)
                {
                    Tile tile = this[x, y, location.Z];
                    if (tile != null)
                    {
                        foreach (var creature in tile.CreatureIds)
                        {
                            result.Add(creature);
                        }
                    }
                }
            }
            return result;
        }

        [Benchmark]
        public List<uint> GetCreaturesAtPositionZoneUsingLinq()
        {
            var result = new List<uint>();

            var toLocation = new Location() { X = 1051, Y = 1090, Z = 7 };
            var location = new Location() { X = 1050, Y = 1090, Z = 7 };

            var viewPortX = (ushort)MapViewPort.ViewPortX;
            var viewPortY = (ushort)MapViewPort.ViewPortY;

            if (location.X != toLocation.X)
            {
                viewPortX++;
            }
            if (location.Y != toLocation.Y)
            {
                viewPortY++;
            }

            var minX = (ushort)(location.X + -viewPortX);
            var minY = (ushort)(location.Y + -viewPortY);
            var maxX = (ushort)(location.X + viewPortX);
            var maxY = (ushort)(location.Y + viewPortY);

            for (ushort x = minX; x <= maxX; x++)
            {
                for (ushort y = minY; y <= maxY; y++)
                {
                    Tile tile = this[x, y, location.Z];
                    if (tile != null)
                    {
                        foreach (var creature in tile.CreatureIds)
                        {
                            result.Add(creature);
                        }
                    }
                }
            }
            return result;
        }
    }
}
