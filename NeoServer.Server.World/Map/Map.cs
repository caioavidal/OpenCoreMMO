// <copyright file="Map.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

using NeoServer.Server.Model.Creatures;
using NeoServer.Server.Model.Items.Contracts;
using NeoServer.Server.Model.Players;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;
using NeoServer.Server.Model.World.Structs;
using NeoServer.Server.World;
using NeoServer.Server.World.Map;
using System;
using System.Collections.Concurrent;
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

        public ConcurrentDictionary<uint, Creature> Creatures { get; }

        public void AddCreature(Player player)
        {
            if (!Creatures.TryAdd(player.CreatureId, player))
            {
                // TODO: proper logging
                Console.WriteLine($"WARNING: Failed to add {player.Name} to the global dictionary.");
            }
        }

        public void AddPlayerOnMap(Player player)
        {
            IThing playerThing = player;

            this[VeteranStart].AddThing(ref playerThing);
        }


        private World _world { get; }

        public Map(World world) { 
            _world = world;
            Creatures = new ConcurrentDictionary<uint, Creature>();
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
                            tempBytes.Add(0xFF);
                        }

                        skip = 0;

                        tempBytes.AddRange(GetTileDescription(player, tile));
                    }
                    else if (skip == 0xFE)
                    {
                        tempBytes.Add(0xFF);
                        tempBytes.Add(0xFF);
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

            var count = 0;
            const int numberOfObjectsLimit = 9;

            if (tile.CachedDescription != null)
            {
                return tile.CachedDescription;
            }

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

                AddItem(tempBytes, item);

                count++;
            }

            foreach (var item in tile.TopItems2)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                AddItem(tempBytes, item);

                count++;
            }


            //todo
            foreach (var creatureId in tile.CreatureIds)
            {
                var creature = Creatures[creatureId];

                if (creature == null)
                {
                    continue;
                }

                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                if (player.KnowsCreatureWithId(creatureId))
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0x62));
                    tempBytes.AddRange(BitConverter.GetBytes(creatureId));
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0x61));
                    tempBytes.AddRange(BitConverter.GetBytes(player.ChooseToRemoveFromKnownSet()));
                    tempBytes.AddRange(BitConverter.GetBytes(creatureId));

                    player.AddKnownCreature(creatureId);

                    var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                    tempBytes.AddRange(creatureNameBytes);
                }

                tempBytes.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
                tempBytes.Add((byte)creature.ClientSafeDirection);

                if (player.CanSee(creature))
                {
                    // Add creature outfit
                    tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.LookType));

                    if (creature.Outfit.LookType > 0)
                    {
                        tempBytes.Add(creature.Outfit.Head);
                        tempBytes.Add(creature.Outfit.Body);
                        tempBytes.Add(creature.Outfit.Legs);
                        tempBytes.Add(creature.Outfit.Feet);
                        tempBytes.Add(creature.Outfit.Addon);
                    }
                    else
                    {
                        tempBytes.AddRange(BitConverter.GetBytes(creature.Outfit.LookType));
                    }
                }
                else
                {
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                    tempBytes.AddRange(BitConverter.GetBytes((ushort)0));
                }

                tempBytes.Add(creature.LightBrightness);
                tempBytes.Add(creature.LightColor);

                tempBytes.AddRange(BitConverter.GetBytes(creature.Speed));

                tempBytes.Add(creature.Skull);
                tempBytes.Add(creature.Shield);

                tempBytes.Add(0x00); //guild emblem

                tempBytes.Add(0x00);

                if (++count == 10)
                {
                    return tempBytes;
                }
            }

            foreach (var item in tile.DownItems)
            {
                if (count == numberOfObjectsLimit)
                {
                    break;
                }

                AddItem(tempBytes, item);
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
