using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

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
            _ => TextColor.None
        };
    }
}
