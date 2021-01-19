namespace NeoServer.Game.Common.Location
{
    public enum TileFlag : byte
    {
        None = 0,
        Refresh = 1 << 0,
        ProtectionZone = 1 << 1,
        NoLogout = 1 << 2
    }
}
