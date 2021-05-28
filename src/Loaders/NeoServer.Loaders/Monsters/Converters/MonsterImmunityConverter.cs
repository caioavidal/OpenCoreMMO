using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Creatures.Model.Monsters;
using System;
using System.Collections.Generic;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterImmunityConverter
    {
        public static ushort Convert(MonsterData data)
        {
            if (data.Immunities is null) return default;

            ushort flag = 0;

            foreach (var immunity in data.Immunities)
            {
                if (immunity.Key.Contains("lifedrain", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.LifeDrain;
                }
                else if (immunity.Key.Contains("paralyze", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Paralysis;
                }
                else if (immunity.Key.Contains("invisible", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Invisibility;
                }
                else if (immunity.Key.Contains("death", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Death;
                }
                else if (immunity.Key.Contains("fire", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Fire;
                }
                else if (immunity.Key.Contains("ice", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Ice;
                }
                else if (immunity.Key.Contains("drown", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Drown;
                }
                else if (immunity.Key.Contains("drunk", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Drunkenness;
                }
                else if (immunity.Key.Contains("earth", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Earth;
                }
                else if (immunity.Key.Contains("energy", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Energy;
                }
                else if (immunity.Key.Contains("poison", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Earth;
                }
                else if (immunity.Key.Contains("physical", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Physical;
                }
                else if (immunity.Key.Contains("holy", StringComparison.InvariantCultureIgnoreCase))
                {
                    flag |= (ushort)ImmunityFlag.Holy;
                }
            }

            return flag;
        }
    }
}
