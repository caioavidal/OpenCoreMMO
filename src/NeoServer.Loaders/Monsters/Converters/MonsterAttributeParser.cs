using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Loaders.Monsters.Converters
{
    public class MonsterAttributeParser
    {
        public static DamageType ParseDamageType(string type)
        {
            return type switch
            {
                "melee" => DamageType.Melee,
                "physical" => DamageType.Physical,
                "energy" => DamageType.Energy,
                "fire" => DamageType.Fire,
                "manadrain" => DamageType.ManaDrain,
                "firearea" => DamageType.Fire,
                "poison" => DamageType.Earth,
                "earth" => DamageType.Earth,
                "bleed"=> DamageType.Physical,
                "drown"=> DamageType.Drown,
                "ice" => DamageType.Ice,
                "holy" => DamageType.Holy,
                "death"=>DamageType.Death,
                "lifedrain"=> DamageType.AbsorbPercentPhysical,
                "mortarea" => DamageType.Death,
                _ => DamageType.Melee
            };
        }
        public static EffectT ParseAreaEffect(string type)
        {
            return type switch
            {
                "blueshimmer" => EffectT.GlitterBlue,
                "redshimmer" => EffectT.GlitterRed,
                "greenshimmer"=> EffectT.GlitterGreen,
                "mortarea" => EffectT.BubbleBlack,
                _ => EffectT.None
            };
        }
     
    }
}
