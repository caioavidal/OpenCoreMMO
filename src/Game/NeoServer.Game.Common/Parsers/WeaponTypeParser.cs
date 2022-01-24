using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Parsers;

public class WeaponTypeParser
{
    public static WeaponType Parse(string weaponType)
    {
        return weaponType switch
        {
            "shield" => WeaponType.Shield,
            "ammunition" => WeaponType.Ammunition,
            "axe" => WeaponType.Axe,
            "club" => WeaponType.Club,
            "sword" => WeaponType.Sword,
            "distance" => WeaponType.Distance,
            "wand" => WeaponType.Magical,
            "rod" => WeaponType.Magical,
            _ => WeaponType.None
        };
    }
}