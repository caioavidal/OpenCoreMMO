using NeoServer.Game.Common;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Parsers.Effects
{
    public class DamageTextColorParser
    {
        public static TextColor Parse(DamageType damageType) => damageType switch
        {
            DamageType.Fire => TextColor.Orange,
            DamageType.Energy => TextColor.Purple,
            DamageType.Melee => TextColor.Red,
            DamageType.Physical => TextColor.Red,
            DamageType.ManaDrain => TextColor.Blue,
            DamageType.FireField => TextColor.Orange,
            DamageType.Earth => TextColor.Green,
            DamageType.Death => TextColor.DarkRed,
            DamageType.AbsorbPercentPhysical => TextColor.DarkRed,
            DamageType.Ice => TextColor.LightBlue,
            DamageType.Holy => TextColor.Yellow,
            _ => TextColor.None
        };
    }
}
