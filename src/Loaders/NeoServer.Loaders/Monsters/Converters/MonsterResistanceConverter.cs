using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Item;

namespace NeoServer.Loaders.Monsters.Converters;

public static class MonsterResistanceConverter
{
    public static IDictionary<DamageType, sbyte> Convert(MonsterData data)
    {
        if (data.Elements is null) return new Dictionary<DamageType, sbyte>(0);

        var immunities = new Dictionary<DamageType, sbyte>(data.Elements.Count);

        foreach (var element in data.Elements)
        {
            var immunity = DamageType.None;
            
            if (element.Key.Contains("energy", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Energy;
            else if (element.Key.Contains("holy", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Holy;
            else if (element.Key.Contains("earth", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Earth;
            else if (element.Key.Contains("death", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Death;
            else if (element.Key.Contains("fire", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Fire;
            else if (element.Key.Contains("ice", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Ice;
            else if (element.Key.Contains("drown", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Drown;
            else if (element.Key.Contains("lifeDrain", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.LifeDrain;
            else if (element.Key.Contains("manaDrain", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.ManaDrain;
            else if (element.Key.Contains("firePercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Fire;
            else if (element.Key.Contains("physical", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Physical;
            else if (element.Key.Contains("icePercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Ice;
            else if (element.Key.Contains("earthPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Earth;
            else if (element.Key.Contains("energyPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Energy;
            else if (element.Key.Contains("holyPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Holy;
            else if (element.Key.Contains("deathPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Death;
            else if (element.Key.Contains("drownPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Drown;
            else if (element.Key.Contains("lifeDrainPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.LifeDrain;
            else if (element.Key.Contains("manaDrainPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.ManaDrain;
            else if (element.Key.Contains("physicalPercent", StringComparison.InvariantCultureIgnoreCase))
                immunity = DamageType.Physical;
            

            if (immunity is DamageType.None)
            {
                Console.WriteLine($"{element.Key} not handled for monster {data.Name}");
            }
            
            immunity = immunity == DamageType.None ? DamageType.Melee : immunity;

            immunities.TryAdd(immunity, element.Value);
        }

        return immunities;
    }
}