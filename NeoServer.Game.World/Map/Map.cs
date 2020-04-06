// <copyright file="Map.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Enums.Location;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Events;
using NeoServer.Server.Contracts;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.World.Map
{

    public class Map : IMap
    {
        private static readonly TimeSpan _mapLoadPercentageReportDelay = TimeSpan.FromSeconds(7);

        // Start positions
        public static Location NewbieStart = new Location { X = 1000, Y = 1000, Z = 7 };
        public static Location VeteranStart = new Location { X = 1000, Y = 1000, Z = 7 };

        public void AddPlayerOnMap(IPlayer player)
        {
            var thing = player as IThing;

            this[player.Location].AddThing(ref thing);            
            dispatcher.Dispatch(new PlayerAddedOnMapEvent(player));
        }

        private readonly World world;
        private readonly CreatureDescription creatureDescription;
        private readonly IDispatcher dispatcher;

        public Map(World world, CreatureDescription creatureDescription, IDispatcher dispatcher)
        {
            this.world = world;
            this.creatureDescription = creatureDescription;
            this.dispatcher = dispatcher;
        }

        public ITile this[Location location] => world.GetTile(location);

        public ITile this[ushort x, ushort y, sbyte z] => this[new Location { X = x, Y = y, Z = z }];

        public void MoveThing(ref IThing thing, Location toLocation, byte count)
        {
            this[thing.Location].RemoveThing(ref thing, count);
            this[toLocation].AddThing(ref thing, count);
        }
        public void RemoveThing(ref IThing thing, ITile tile, byte count)
        {
            var stackPosition = thing.GetStackPosition();
            tile.RemoveThing(ref thing, count);

            dispatcher.Dispatch(new ThingRemovedFromTileEvent(tile, stackPosition));
        }


        public IEnumerable<uint> GetCreaturesAtPositionZone(Location location)
        {
            var minX = (ushort)(location.X + MapViewPort.MinViewPortX);
            var minY = (ushort)(location.Y + MapViewPort.MinViewPortY);
            var maxX = (ushort)(location.X + MapViewPort.MaxViewPortX);
            var maxY = (ushort)(location.Y + MapViewPort.MaxViewPortY);

            for (ushort x = minX; x <= maxX; x++)
            {
                for (ushort y = minY; y <= maxY; y++)
                {
                    ITile tile = this[x, y, location.Z];
                    if (tile != null)
                    {
                        foreach (var creature in tile.CreatureIds)
                        {
                            yield return creature;
                        }
                    }
                }
            }
        }

        public IEnumerable<ITile> GetOffsetTiles(Location location)
        {
            var fromX = location.X - 8;
            var fromY = location.Y - 6;

            var toX = location.X + 8;
            var toY = location.Y + 6;

            for (var x = fromX; x <= toX; x++)
            {
                for (var y = fromY; y <= toY; y++)
                {
                    var tile = this[(ushort)x, (ushort)y, location.Z];
                    yield return tile;
                }
            }
        }

        public IList<byte> GetDescription(IThing thing, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = MapConstants.DefaultMapWindowSizeX, byte windowSizeY = MapConstants.DefaultMapWindowSizeY)
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
                tempBytes.AddRange(GetFloorDescription(thing, fromX, fromY, (sbyte)nz, windowSizeX, windowSizeY, currentZ - nz, ref skip));
            }

            if (skip >= 0)
            {
                tempBytes.Add((byte)skip);
                tempBytes.Add(0xFF);
            }

            return tempBytes;
        }

        private IList<byte> GetFloorDescription(IThing thing, ushort fromX, ushort fromY, sbyte currentZ, byte width, byte height, int verticalOffset, ref int skip)
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

                        tempBytes.AddRange(GetTileDescription(thing, tile));
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

        private IList<byte> GetTileDescription(IThing thing, ITile tile)
        {
            if (tile == null)
            {
                return new byte[0];
            }



            var tempBytes = new List<byte>();

            var objectsOnTile = 0;

            if (tile.CachedDescription != null)
            {
                return tile.CachedDescription;
            }


            if (tile.Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(tile.Ground.Type.ClientId));
                objectsOnTile++;
            }



            foreach (var item in tile.TopItems1)
            {

                if (objectsOnTile == MapConstants.LimitOfObjectsOnTile)
                {
                    break;
                }

                AddItem(tempBytes, item);

                objectsOnTile++;
            }

            foreach (var item in tile.TopItems2)
            {
                if (objectsOnTile == MapConstants.LimitOfObjectsOnTile)
                {
                    break;
                }

                AddItem(tempBytes, item);

                objectsOnTile++;
            }

            tempBytes.AddRange(creatureDescription.GetCreaturesOnTile(thing, tile, ref objectsOnTile));

            foreach (var item in tile.DownItems)
            {
                if (objectsOnTile == MapConstants.LimitOfObjectsOnTile)
                {
                    break;
                }

                AddItem(tempBytes, item);

                objectsOnTile++;
            }

            return tempBytes;
        }

        private void AddItem(List<byte> tempBytes, IItem item)
        {
            tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

            if (item.IsCumulative)
            {
                tempBytes.Add(item.Amount);
            }
            else if (item.IsLiquidPool || item.IsLiquidContainer)
            {
                tempBytes.Add(item.LiquidType);
            }
        }
    }
}
