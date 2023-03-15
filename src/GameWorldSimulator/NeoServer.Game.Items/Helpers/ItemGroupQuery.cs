using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Helpers;

internal static class ItemGroupQuery
{
    public static ItemGroup Find(ItemType metadata)
    {
        return GetWeaponGroup(metadata);
    }

    private static ItemGroup GetWeaponGroup(IItemType metadata)
    {
        if (metadata.WeaponType is WeaponType.Axe or WeaponType.Club or WeaponType.Sword) return ItemGroup.MeleeWeapon;

        if (metadata.Attributes.GetAttribute(ItemAttribute.WeaponType) is "wand" or "rod") return ItemGroup.MagicWeapon;

        if (metadata.Attributes.GetAttribute(ItemAttribute.WeaponType) == "distance")
        {
            return metadata.HasFlag(ItemFlag.Stackable) ? ItemGroup.ThrowableDistanceWeapon : ItemGroup.DistanceWeapon;
        }

        if (metadata.WeaponType == WeaponType.Ammunition) return ItemGroup.Ammo;

        return ItemGroup.None;
    }
}