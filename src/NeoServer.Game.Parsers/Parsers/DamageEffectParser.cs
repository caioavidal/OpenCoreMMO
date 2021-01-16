using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common.Item;

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
            DamageType.Death => EffectT.BubbleBlack,
            DamageType.Holy => EffectT.HolyDamage,
            DamageType.Ice => EffectT.IceAttack,
            _ => EffectT.None
        };
    }
}
