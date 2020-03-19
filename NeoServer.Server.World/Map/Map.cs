// <copyright file="Map.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using NeoServer.Server.Model.World.Structs;
using NeoServer.Server.World;
using NeoServer.Server.World.Map;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.World.Map
{
    public class Map
    {
        private static readonly TimeSpan _mapLoadPercentageReportDelay = TimeSpan.FromSeconds(7);

        // Start positions
        public static Location NewbieStart = new Location { X = 1000, Y = 1000, Z = 7 };
        public static Location VeteranStart = new Location { X = 1000, Y = 1000, Z = 7 };
		
        private World _world { get; }

        public Map(World world)  { _world = world; }
		
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

        public IList<byte> GetDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, bool isUnderground, byte windowSizeX = MapConstants.DefaultMapWindowSizeX, byte windowSizeY = MapConstants.DefaultMapWindowSizeY)
        {
            var tempBytes = new List<byte>();

            var skip = -1;

            // we crawl from the ground up to the very top of the world (7 -> 0).
            var crawlTo = 0;
            var crawlFrom = 7;
            var crawlDelta = -1;

            // Unless... we're undeground.
            // Then we crawl from 2 floors up, this, and 2 floors down for a total of 5 floors.
            if (isUnderground)
            {
                crawlDelta = 1;
                crawlFrom = Math.Max(0, currentZ - 2);
                crawlTo = Math.Min(15, currentZ + 2);
            }
			
			for (var z = crawlFrom; z != crawlTo + crawlDelta; z += crawlDelta)
            {
                tempBytes.AddRange(GetFloorDescription(player, fromX, fromY, (sbyte)z, windowSizeX, windowSizeY, currentZ - z, ref skip));
            }

            if (skip >= 0)
            {
                tempBytes.Add((byte)skip);
                tempBytes.Add(0xFF);
            }

            return tempBytes;
        }

        public IList<byte> GetFloorDescription(IPlayer player, ushort fromX, ushort fromY, sbyte currentZ, byte windowSizeX, byte windowSizeY, int verticalOffset, ref int skip)
        {
            var tempBytes = new List<byte>();

            for (var nx = 0; nx < windowSizeX; nx++)
            {
                for (var ny = 0; ny < windowSizeY; ny++)
                {
                    var tile = this[(ushort)(fromX + nx + verticalOffset), (ushort)(fromY + ny + verticalOffset), currentZ];

                    if (tile != null)
                    {
                        if (skip >= 0)
                        {
                            tempBytes.Add((byte)skip);
                            tempBytes.Add(0xFF);
                        }

                        skip = 0;

                        tempBytes.AddRange(GetTileDescription(player, tile));
                    }
                    else if (++skip == 0xFF)
                    {
                        tempBytes.Add(0xFF);
                        tempBytes.Add(0xFF);
                        skip = -1;
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

			if (tile.CachedDescription != null) {
				return tile.CachedDescription;
			}

			var tempBytes = new List<byte>();

            var count = 0;
            const int numberOfObjectsLimit = 9;

            if (tile.Ground != null)
            {
                tempBytes.AddRange(BitConverter.GetBytes(tile.Ground.Type.ClientId));
                count++;
            }

            foreach (var item in tile.TopItems1)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            foreach (var item in tile.TopItems2)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            //todo
            //foreach (var creatureId in tile.CreatureIds)
            //{
            //    var creature = Game.Instance.GetCreatureWithId(creatureId);

            //    if (creature == null)
            //    {
            //        continue;
            //    }

            //    if (count == numberOfObjectsLimit)
            //    {
            //        break;
            //    }

            //    if (player.KnowsCreatureWithId(creatureId))
            //    {
            //        tempBytes.AddRange(BitConverter.GetBytes((ushort)GameOutgoingPacketType.AddKnownCreature));
            //        tempBytes.AddRange(BitConverter.GetBytes(creatureId));
            //    }
            //    else
            //    {
            //        tempBytes.AddRange(BitConverter.GetBytes((ushort)GameOutgoingPacketType.AddUnknownCreature));
            //        tempBytes.AddRange(BitConverter.GetBytes(player.ChooseToRemoveFromKnownSet()));
            //        tempBytes.AddRange(BitConverter.GetBytes(creatureId));

            //        player.AddKnownCreature(creatureId);

            //        var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
            //        tempBytes.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
            //        tempBytes.AddRange(creatureNameBytes);
            //    }

            //    tempBytes.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
            //    tempBytes.Add((byte)creature.ClientSafeDirection);

            //    if (player.CanSee(creature))
            //    {
            //        // Add creature outfit
            //        tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.Id));

            //        if (creature.Outfit.Id > 0)
            //        {
            //            tempBytes.Add(creature.Outfit.Head);
            //            tempBytes.Add(creature.Outfit.Body);
            //            tempBytes.Add(creature.Outfit.Legs);
            //            tempBytes.Add(creature.Outfit.Feet);
            //        }
            //        else
            //        {
            //            tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.LikeType));
            //        }
            //    }
            //    else
            //    {
            //        tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
            //        tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
            //    }

            //    tempBytes.Add(creature.LightBrightness);
            //    tempBytes.Add(creature.LightColor);

            //    tempBytes.AddRange(BitConverter.GetBytes(creature.Speed));

            //    tempBytes.Add(creature.Skull);
            //    tempBytes.Add(creature.Shield);
            //}

            foreach (var item in tile.DownItems)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                tempBytes.AddRange(BitConverter.GetBytes(item.Type.ClientId));

                if (item.IsCumulative)
                {
                    tempBytes.Add(item.Amount);
                }
                else if (item.IsLiquidPool || item.IsLiquidContainer)
                {
                    tempBytes.Add(item.LiquidType);
                }

                count++;
            }

            return tempBytes;
        }
    }
}
