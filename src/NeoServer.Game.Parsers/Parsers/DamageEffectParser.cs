using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Parsers.Effects
{
    public class DamageEffectParser
    {
        public static EffectT Parse(DamageType damageType) => damageType switch
        {
            DamageType.Fire => EffectT.AreaFlame,
            DamageType.FireField => EffectT.Flame,
            DamageType.Energy => EffectT.DamageEnergy,
            DamageType.Melee => EffectT.XBlood,
            DamageType.Physical => EffectT.XBlood,
            DamageType.Earth => EffectT.RingsGreen,
            DamageType.AbsorbPercentPhysical => EffectT.GlitterRed,
            DamageType.ManaDrain => EffectT.GlitterRed,
            _ => EffectT.None
        };
    }
}
