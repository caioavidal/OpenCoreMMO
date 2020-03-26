// <copyright file="Map.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Item;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.World.Map
{
   
    public class Map : IMap
    {
        private static readonly TimeSpan _mapLoadPercentageReportDelay = TimeSpan.FromSeconds(7);

        // Start positions
        public static Location NewbieStart = new Location { X = 1000, Y = 1000, Z = 7 };
        public static Location VeteranStart = new Location { X = 1000, Y = 1000, Z = 7 };
        //public static Location VeteranStart = new Location { X = 490, Y = 171, Z = 7 };

        public void AddPlayerOnMap(IPlayer player)
        {
            IThing playerThing = player;

            this[VeteranStart].AddThing(ref playerThing);
        }


        private readonly World _world;
        private readonly CreatureDescription _creatureDescription;

        public Map(World world, CreatureDescription creatureDescription)
        {
            _world = world;
            _creatureDescription = creatureDescription;
        }

        public ITile this[Location location] => _world.GetTile(location);

        public ITile this[ushort x, ushort y, sbyte z] => this[new Location { X = x, Y = y, Z = z }];

        internal IEnumerable<uint> GetCreatureIdsAt(Location location)
        {
            var fromX = location.X - 8;
            var fromY = location.Y - 6;

            var toX = location.X + 8;
            var toY = location.Y + 6;

            var creatureList = new List<uint>();

            for (var x = fromX; x <= toX; x++)
            {
                for (var y = fromY; y <= toY; y++)
                {
                    var creaturesInTile = this[(ushort)x, (ushort)y, location.Z]?.CreatureIds;

                    if (creaturesInTile != null)
                    {
                        creatureList.AddRange(creaturesInTile);
                    }
                }
            }

            return creatureList;
        }

        public IEnumerable<ITile> GetTilesNear(Location location)
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

        public IList<byte> GetDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = MapConstants.DefaultMapWindowSizeX, byte windowSizeY = MapConstants.DefaultMapWindowSizeY)
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
                tempBytes.AddRange(GetFloorDescription(player, fromX, fromY, (sbyte)nz, windowSizeX, windowSizeY, currentZ - nz, ref skip));
            }

            if (skip >= 0)
            {
                tempBytes.Add((byte)skip);
                tempBytes.Add(0xFF);
            }

            return tempBytes;
        }

        public IList<byte> GetFloorDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte width, byte height, int verticalOffset, ref int skip)
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

                        tempBytes.AddRange(GetTileDescription(player, tile));
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

        public IList<byte> GetTileDescription(IPlayer player, ITile tile)
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

            tempBytes.AddRange(_creatureDescription.GetCreaturesOnTile(player, tile, ref objectsOnTile));

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
