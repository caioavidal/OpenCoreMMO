using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Texts;

namespace NeoServer.Game.Common.Effects.Parsers;

public class DamageTextColorParser
{
    public static TextColor Parse(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Fire => TextColor.Orange,
            DamageType.Energy => TextColor.Purple,
            DamageType.Melee => TextColor.Red,
            DamageType.Physical => TextColor.Red,
            DamageType.ManaDrain => TextColor.Blue,
            DamageType.FireField => TextColor.Orange,
            DamageType.Earth => TextColor.Green,
            DamageType.Death => TextColor.DarkRed,
            DamageType.LifeDrain => TextColor.DarkRed,
            DamageType.Ice => TextColor.LightBlue,
            DamageType.Holy => TextColor.Yellow,
            _ => TextColor.None
        };
    }
}