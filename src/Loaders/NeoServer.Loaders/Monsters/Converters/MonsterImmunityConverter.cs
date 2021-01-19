using NeoServer.Game.Common.Item;
using System;
using System.Collections.Generic;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterImmunityConverter
    {
        public static IDictionary<DamageType, sbyte> Convert(MonsterData data)
        {
            if(data.Elements is null) return new Dictionary<DamageType, sbyte>(0);

            var immunities = new Dictionary<DamageType, sbyte>(data.Elements.Count);

            foreach (var element in data.Elements)
            {
                DamageType immunity = DamageType.Melee;
                if (element.Key.Contains("energy", StringComparison.InvariantCultureIgnoreCase))
                {
                    immunity = DamageType.Energy;
                }
                else if (element.Key.Contains("holy", StringComparison.InvariantCultureIgnoreCase))
                {
                    immunity = DamageType.Holy;
                }
                else if (element.Key.Contains("earth", StringComparison.InvariantCultureIgnoreCase))
                {
                    immunity = DamageType.Earth;
                }
                else if (element.Key.Contains("death", StringComparison.InvariantCultureIgnoreCase))
                {
                    immunity = DamageType.Death;
                }
                else if (element.Key.Contains("fire", StringComparison.InvariantCultureIgnoreCase))
                {
                    immunity = DamageType.Fire;
                }
                else if (element.Key.Contains("ice", StringComparison.InvariantCultureIgnoreCase))
                {
                    immunity = DamageType.Ice;
                }

                immunities.Add(immunity, element.Value);
            }

            return immunities;
        }
    }
}
