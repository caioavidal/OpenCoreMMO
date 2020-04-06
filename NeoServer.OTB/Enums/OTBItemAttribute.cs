namespace NeoServer.OTB.Enums
{
    public enum OTBItemAttribute : byte
    {
        ServerId = 0x10,
        ClientId,
        Name,
        Description,
        Speed,
        Slot,
        MaxItems,
        Weight,
        Weapon,
        Ammunition,
        Armor,
        MagicLevel,
        MagicFieldType,
        Writeable,
        RotateTo,
        Decay,
        SpriteHash,
        MiniMapColor,
        Attr07,
        Attr08,
        Light,

        //1-byte aligned
        Decay2, //deprecated
        Weapon2, //deprecated
        Ammunition2, //deprecated
        Armor2, //deprecated
        Writeable2, //deprecated
        Light2,
        TopOrder,
        Writeable3, //deprecated

        WareId,
        LightLevel,
        LightColor
    }
}

