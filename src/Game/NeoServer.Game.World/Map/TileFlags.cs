namespace NeoServer.Game.World
{

    /// <summary>
    /// This enum is meant to store in a memory-efficient way a collection
    /// of properties that a Tile can have.
    /// </summary>
    public enum TileFlags : uint
    {
        None = 0b000000000000000000000000,
        FloorChangeDown = 0b000000000000000000000001,
        FloorChangeNorth = 0b000000000000000000000010,
        FloorChangeSouth = 0b000000000000000000000100,
        FloorChangeEast = 0b000000000000000000001000,
        FloorChangeWest = 0b000000000000000000010000,
        FloorChangeSouthAlternative = 0b000000000000000000100000,
        FloorChangeEastAlternative = 0b000000000000000001000000,
        ProtectionZone = 0b000000000000000010000000,
        NoPvpZone = 0b000000000000000100000000,
        NoLogout = 0b000000000000001000000000,
        PvpZone = 0b000000000000010000000000,
        Teleport = 0b000000000000100000000000,
        MagicField = 0b000000000001000000000000,
        MailBox = 0b000000000010000000000000,
        TrashHolder = 0b000000000100000000000000,
        Bed = 0b000000001000000000000000,
        Depot = 0b000000010000000000000000,
        BlockSolid = 0b000000100000000000000000,
        BlockPath = 0b000001000000000000000000,
        ImmovableBlockSolid = 0b000010000000000000000000,
        ImmovableBlockPath = 0b000100000000000000000000,
        ImmovableFieldBlockPath = 0b001000000000000000000000,
        NoFieldBlockPath = 0b010000000000000000000000,
        SupportsHangable = 0b100000000000000000000000
    }
}