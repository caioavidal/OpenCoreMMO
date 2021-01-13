using System;
namespace NeoServer.OTBM.Enums
{

    /// <summary>
    /// This is a port TFS's enum "ItemType" definied in const.h, line 435
    /// Used to parse OTBM worlds.
    /// </summary>
    public enum OTBMWorldItemId : UInt16
    {
        BrowseField = 460,                  // ITEM_BROWSEFIELD = 460, // for internal use

        FireFieldPvpLarge = 1487,           // ITEM_FIREFIELD_PVP_FULL = 1487,
        FireFieldPvpMedium = 1488,          // ITEM_FIREFIELD_PVP_MEDIUM = 1488,
        FireFieldPvpSmall = 1489,           // ITEM_FIREFIELD_PVP_SMALL = 1489,
        FireFieldPersistentLarge = 1492,    // ITEM_FIREFIELD_PERSISTENT_FULL = 1492,
        FireFieldPersistentMedium = 1493,   // ITEM_FIREFIELD_PERSISTENT_MEDIUM = 1493,
        FireFieldPersistentSmall = 1494,    // ITEM_FIREFIELD_PERSISTENT_SMALL = 1494,
        FireFieldNoPvp = 1500,              // ITEM_FIREFIELD_NOPVP = 1500,

        PoisonFieldPvp = 1490,              // ITEM_POISONFIELD_PVP = 1490,
        PoisonFieldPersistent = 1496,       // ITEM_POISONFIELD_PERSISTENT = 1496,
        PoisonFieldNoPvp = 1503,            // ITEM_POISONFIELD_NOPVP = 1503,

        EnergyFieldPvp = 1491,              // ITEM_ENERGYFIELD_PVP = 1491,
        EnergyFieldPersistent = 1495,       // ITEM_ENERGYFIELD_PERSISTENT = 1495,
        EnergyFieldNoPvp = 1504,            // ITEM_ENERGYFIELD_NOPVP = 1504,

        MagicWall = 1497,                   // ITEM_MAGICWALL = 1497,
        MagicWallPersistent = 1498,         // ITEM_MAGICWALL_PERSISTENT = 1498,
        MagicWallSafe = 11098,              // ITEM_MAGICWALL_SAFE = 11098,

        WildGrowth = 1499,                  // ITEM_WILDGROWTH = 1499,
        WildGrowthPersistent = 2721,        // ITEM_WILDGROWTH_PERSISTENT = 2721,
        WildGrowthSafe = 11099,             // ITEM_WILDGROWTH_SAFE = 11099,

        Bag = 1987,                         // ITEM_BAG = 1987,

        GoldCoin = 2148,                    // ITEM_GOLD_COIN = 2148,
        PlatinumCoin = 2152,                // ITEM_PLATINUM_COIN = 2152,
        CrystalCoin = 2160,                 // ITEM_CRYSTAL_COIN = 2160,

        Depot = 2594,                       // ITEM_DEPOT = 2594,
        Locker = 2589,                      // ITEM_LOCKER1 = 2589,
        Inbox = 14404,                      // ITEM_INBOX = 14404,
        Market = 14405,                     // ITEM_MARKET = 14405,

        MaleCorpse = 3058,                  // ITEM_MALE_CORPSE = 3058,
        FemaleCorpse = 3065,                // ITEM_FEMALE_CORPSE = 3065,

        LargeSplash = 2016,                 // ITEM_FULLSPLASH = 2016,
        SmallSplash = 2019,                 // ITEM_SMALLSPLASH = 2019,

        Parcel = 2595,                      // ITEM_PARCEL = 2595,
        Letter = 2597,                      // ITEM_LETTER = 2597,
        LetterStamped = 2598,               // ITEM_LETTER_STAMPED = 2598,
        Label = 2599,                       // ITEM_LABEL = 2599,

        AmuletOfLoss = 2173,                // ITEM_AMULETOFLOSS = 2173,

        DocumentReadOnly = 1968             // ITEM_DOCUMENT_RO = 1968        , //read-only
    }
}

