using System;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Items.Helpers;

internal static class ItemGroupQuery
{
    public static ItemGroup Find(ItemType metadata)
    {
        if (Guard.IsNull(metadata)) return ItemGroup.None;

        var queries = new[]
        {
            GetWeaponGroup,
            GetDefenseEquipmentGroup,
            GetFloorChangeGroup,
            GetMagicFieldGroup,
            GetPaperGroup,
            GetSignGroup,
            GetTeleportGroup
        };

        return GetItemGroup(metadata, queries);
    }

    private static ItemGroup GetItemGroup(IItemType metadata, Func<IItemType, ItemGroup>[] queries)
    {
        foreach (var query in queries)
        {
            var itemGroup = query.Invoke(metadata);
            if (itemGroup is not ItemGroup.None) return itemGroup;
        }

        return ItemGroup.None;
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

    private static ItemGroup GetDefenseEquipmentGroup(IItemType metadata)
    {
        return metadata.BodyPosition switch
        {
            Slot.Body => true,
            Slot.Legs => true,
            Slot.Head => true,
            Slot.Feet => true,
            Slot.Right => true,
            Slot.Ring => true,
            Slot.Necklace => true,
            _ => false
        } || metadata.WeaponType == WeaponType.Shield
            ? ItemGroup.BodyDefenseEquipment
            : ItemGroup.None;
    }

    private static ItemGroup GetFloorChangeGroup(IItemType metadata) =>
        metadata.Attributes.HasAttribute(ItemAttribute.FloorChange) && metadata.HasFlag(ItemFlag.Useable)
            ? ItemGroup.FloorChanger
            : ItemGroup.None;

    private static ItemGroup GetMagicFieldGroup(IItemType metadata) =>
        metadata.Attributes.GetAttribute(ItemAttribute.Type) == "magicfield" ? ItemGroup.MagicField : ItemGroup.None;

    private static ItemGroup GetPaperGroup(IItemType metadata) =>
        metadata.Flags.Contains(ItemFlag.Readable) ? ItemGroup.Paper : ItemGroup.None;

    private static ItemGroup GetSignGroup(IItemType metadata) =>
        (metadata.Attributes.HasAttribute(ItemAttribute.Text) && !metadata.Flags.Contains(ItemFlag.Useable)) ||
        metadata.Attributes.GetAttribute(ItemAttribute.Type) == "sign"
            ? ItemGroup.Sign
            : ItemGroup.None;

    private static ItemGroup GetTeleportGroup(IItemType metadata) =>
        metadata.Attributes.GetAttribute(ItemAttribute.Type)
            ?.Equals("teleport", StringComparison.InvariantCultureIgnoreCase) ?? false
            ? ItemGroup.Teleport
            : ItemGroup.None;
    
    private static ItemGroup GetContainerGroup(IItemType metadata) =>
        metadata.Attributes.GetAttribute(ItemAttribute.Type)
            ?.Equals("teleport", StringComparison.InvariantCultureIgnoreCase) ?? false
            ? ItemGroup.Teleport
            : ItemGroup.None;
}