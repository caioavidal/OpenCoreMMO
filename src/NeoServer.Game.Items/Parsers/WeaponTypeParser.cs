using NeoServer.Game.Enums.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Items.Parsers
{
    internal class WeaponTypeParser
    {
        public static WeaponType Parse(string weaponType) => weaponType switch
        {
            "shield" => WeaponType.Shield,
            "ammunition" => WeaponType.Ammunition,
            "axe" => WeaponType.Axe,
            "club" => WeaponType.Club,
            "sword" => WeaponType.Sword,
            "distance" => WeaponType.Distance,
            "wand" => WeaponType.Wand,
            _ => WeaponType.None
        };
    }
}
