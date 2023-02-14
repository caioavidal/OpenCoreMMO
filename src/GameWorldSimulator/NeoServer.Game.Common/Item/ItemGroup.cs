namespace NeoServer.Game.Common.Item;

public enum ItemGroup : byte
{
    None,

    Ground,
    GroundContainer,
    Weapon, //deprecated
    Ammunition, //deprecated
    ItemGroupArmor, //deprecated
    ItemGroupCharges,
    ItemGroupTeleport, //deprecated
    ItemGroupMagicField, //deprecated
    ItemGroupWriteable, //deprecated
    ItemGroupKey, //deprecated
    Splash,
    ItemGroupFluid,
    ItemGroupDoor, //deprecated
    ItemGroupDeprecated,

    ItemGroupLast
}