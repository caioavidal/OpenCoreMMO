using System;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterImmunityConverter
    {
        public static ushort Convert(MonsterData data)
        {
            if (data.Immunities is null) return default;

            ushort flag = 0;

            foreach (var immunity in data.Immunities)
                if (immunity.Key.Contains("lifedrain", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.LifeDrain;
                else if (immunity.Key.Contains("paralyze", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Paralysis;
                else if (immunity.Key.Contains("invisible", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Invisibility;
                else if (immunity.Key.Contains("death", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Death;
                else if (immunity.Key.Contains("fire", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Fire;
                else if (immunity.Key.Contains("ice", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Ice;
                else if (immunity.Key.Contains("drown", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Drown;
                else if (immunity.Key.Contains("drunk", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Drunkenness;
                else if (immunity.Key.Contains("earth", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Earth;
                else if (immunity.Key.Contains("energy", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Energy;
                else if (immunity.Key.Contains("poison", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Earth;
                else if (immunity.Key.Contains("physical", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Physical;
                else if (immunity.Key.Contains("holy", StringComparison.InvariantCultureIgnoreCase))
                    flag |= (ushort) Immunity.Holy;

            return flag;
        }
    }
}