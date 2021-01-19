namespace NeoServer.Game.Common
{
    public enum ItemGroup : byte
    {
        None,

        Ground,
        GroundContainer,
        Weapon, //deprecated
        Ammunition, //deprecated
        ITEM_GROUP_ARMOR, //deprecated
        ITEM_GROUP_CHARGES,
        ITEM_GROUP_TELEPORT, //deprecated
        ITEM_GROUP_MAGICFIELD, //deprecated
        ITEM_GROUP_WRITEABLE, //deprecated
        ITEM_GROUP_KEY, //deprecated
        Splash,
        ITEM_GROUP_FLUID,
        ITEM_GROUP_DOOR, //deprecated
        ITEM_GROUP_DEPRECATED,

        ITEM_GROUP_LAST
    }
}
