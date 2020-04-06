using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.World.Map;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Model.World.Map;

namespace NeoServer.Game.World.Map
{
    public class CreatureDescription
    {
        private readonly ICreatureGameInstance _creatureInstances;
        public CreatureDescription(ICreatureGameInstance creatureInstances)
        {
            _creatureInstances = creatureInstances;
        }

        public List<byte> GetCreaturesOnTile(IThing thing, ITile tile, ref int objectsOnTile)
        {
            var stream = new List<byte>();


            foreach (var creatureId in tile.CreatureIds)
            {
                if (objectsOnTile == MapConstants.LimitOfObjectsOnTile)
                {
                    return stream;
                }

                var creature = _creatureInstances[creatureId];

                if (creature == null)
                {
                    continue;
                }

                var player = thing as IPlayer;

                var known = player.KnowsCreatureWithId(creatureId);

                if (known)
                {
                    stream.AddRange(BitConverter.GetBytes((ushort)0x62));
                    stream.AddRange(BitConverter.GetBytes(creatureId));
                }
                else
                {
                    stream.AddRange(BitConverter.GetBytes((ushort)0x61));
                    stream.AddRange(BitConverter.GetBytes(player.ChooseToRemoveFromKnownSet()));
                    stream.AddRange(BitConverter.GetBytes(creatureId));

                    player.AddKnownCreature(creatureId);

                    var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                    stream.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                    stream.AddRange(creatureNameBytes);
                }

                stream.Add((byte)Math.Min(100, creature.Hitpoints * 100 / creature.MaxHitpoints));
                stream.Add((byte)creature.ClientSafeDirection);

                if (player.CanSee(creature))
                {
                    // Add creature outfit
                    stream.AddRange(BitConverter.GetBytes(creature.Outfit.LookType));

                    if (creature.Outfit.LookType > 0)
                    {
                        stream.Add(creature.Outfit.Head);
                        stream.Add(creature.Outfit.Body);
                        stream.Add(creature.Outfit.Legs);
                        stream.Add(creature.Outfit.Feet);
                        stream.Add(creature.Outfit.Addon);
                    }
                    else
                    {
                        stream.AddRange(BitConverter.GetBytes(creature.Outfit.LookType));
                    }
                }
                else
                {
                    stream.AddRange(BitConverter.GetBytes((ushort)0));
                    stream.AddRange(BitConverter.GetBytes((ushort)0));
                }

                stream.Add(creature.LightBrightness);
                stream.Add(creature.LightColor);

                stream.AddRange(BitConverter.GetBytes(creature.Speed));

                stream.Add(creature.Skull);
                stream.Add(creature.Shield);

                if (!known)
                {
                    stream.Add(0x00); //guild emblem
                }

                stream.Add(0x00);

                objectsOnTile++;
            }

            return stream;
        }
    }
}