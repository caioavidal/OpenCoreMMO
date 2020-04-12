namespace NeoServer.OTBM.Enums
{

    /// <summary>
    /// This enum is used to identify the contents of a OTBNode used to describe
    /// world nodes.
    /// </summary>
    public enum NodeAttribute : byte
    {
        None = 0,
        WorldDescription = 1,                   // OTBM_ATTR_DESCRIPTION = 1,
        ExtensionFile = 2,                      // OTBM_ATTR_EXT_FILE = 2,
        TileFlags = 3,                          // OTBM_ATTR_TILE_FLAGS = 3,
        ActionId = 4,                           // OTBM_ATTR_ACTION_ID = 4,
        UniqueId = 5,                           // OTBM_ATTR_UNIQUE_ID = 5,
        Text = 6,                               // OTBM_ATTR_TEXT = 6,
        NotTheMapDescription = 7,               // OTBM_ATTR_DESC = 7,
        TeleportDestination = 8,                // OTBM_ATTR_TELE_DEST = 8,
        Item = 9,                               // OTBM_ATTR_ITEM = 9,
        DepotId = 10,                           // OTBM_ATTR_DEPOT_ID = 10,
        ExtensionFileForSpawns = 11,            // OTBM_ATTR_EXT_SPAWN_FILE = 11,
        RuneCharges = 12,                       // OTBM_ATTR_RUNE_CHARGES = 12,
        ExtensionFileForHouses = 13,            // OTBM_ATTR_EXT_HOUSE_FILE = 13,
        HouseDoorId = 14,                       // OTBM_ATTR_HOUSEDOORID = 14,
        Count = 15,                             // OTBM_ATTR_COUNT = 15,
        Duration = 16,                          // OTBM_ATTR_DURATION = 16,
        DecayingState = 17,                     // OTBM_ATTR_DECAYING_STATE = 17,
        WrittenDate = 18,                       // OTBM_ATTR_WRITTENDATE = 18,
        WrittenBy = 19,                         // OTBM_ATTR_WRITTENBY = 19,
        SleeperGuid = 20,                       // OTBM_ATTR_SLEEPERGUID = 20,
        SleepStart = 21,                        // OTBM_ATTR_SLEEPSTART = 21,
        Charges = 22                            // OTBM_ATTR_CHARGES = 22,
    }
}
