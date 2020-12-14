namespace NeoServer.Enums.Creatures.Enums
{
    public enum EffectT : byte
    {
        XBlood = 0x01,
        RingsBlue = 0x02,
        Puff = 0x03,
        SparkYellow = 0x04,
        DamageExplosion = 0x05,
        DamageMagicMissile = 0x06,
        AreaFlame = 0x07,
        RingsYellow = 0x08,
        RingsGreen = 0x09,
        XGray = 0x0A,
        BubbleBlue = 0x0B,
        DamageEnergy = 0x0C,
        GlitterBlue = 0x0D,
        GlitterRed = 0x0E,
        GlitterGreen = 0x0F,
        Flame = 0x10,
        XPoison = 0x11,
        BubbleBlack = 0x12,
        SoundGreen = 0x13,
        SoundRed = 0x14,
        DamageVenomMissile = 0x15,
        SoundYellow = 0x16,
        SoundPurple = 0x17,
        SoundBlue = 0x18,
        SoundWhite = 0x19,
        GroundShaker = 35,
        None = 0xFF // Don't send to client.
    }
}
