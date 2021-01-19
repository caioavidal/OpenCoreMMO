#define GAME_FEATURE_MESSAGE_LEVEL
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures
{
    public class CreatureRaw
    {
        public static byte[] Convert(IPlayer playerRequesting, IWalkableCreature creature)
        {
            var cache = new List<byte>();

            var known = playerRequesting.KnowsCreatureWithId(creature.CreatureId);

            if (known)
            {
                cache.AddRange(BitConverter.GetBytes((ushort)0x62));
                cache.AddRange(BitConverter.GetBytes(creature.CreatureId));
            }
            else
            {
                cache.AddRange(BitConverter.GetBytes((ushort)0x61));
                cache.AddRange(BitConverter.GetBytes(playerRequesting.ChooseToRemoveFromKnownSet()));
                cache.AddRange(BitConverter.GetBytes(creature.CreatureId));

                var creatureNameBytes = Encoding.Default.GetBytes(creature.Name);
                cache.AddRange(BitConverter.GetBytes((ushort)creatureNameBytes.Length));
                cache.AddRange(creatureNameBytes);
            }

            cache.Add((byte)Math.Min(100, creature.HealthPoints * 100 / creature.MaxHealthPoints));
            cache.Add((byte)creature.ClientSafeDirection);

            if (playerRequesting.CanSee(creature))
            {
                // Add creature outfit
                cache.AddRange(BitConverter.GetBytes(creature.Outfit.LookType));

                if (creature.Outfit.LookType > 0)
                {
                    cache.Add(creature.Outfit.Head);
                    cache.Add(creature.Outfit.Body);
                    cache.Add(creature.Outfit.Legs);
                    cache.Add(creature.Outfit.Feet);
                    cache.Add(creature.Outfit.Addon);
                }
                else
                {
                    cache.AddRange(BitConverter.GetBytes(creature.Outfit.LookType));
                }
            }
            else
            {
                cache.AddRange(BitConverter.GetBytes((ushort)0));
                cache.AddRange(BitConverter.GetBytes((ushort)0));
            }

            cache.Add(creature.LightBrightness);
            cache.Add(creature.LightColor);

            cache.AddRange(BitConverter.GetBytes(creature.Speed));

            cache.Add(creature.Skull);
            cache.Add(creature.Shield);

            if (!known)
            {
                cache.Add(0x00); //guild emblem
            }

            cache.Add(0x01);
            return cache.ToArray();
        }
    }
}
