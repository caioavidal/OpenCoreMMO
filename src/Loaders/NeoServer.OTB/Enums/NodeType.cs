namespace NeoServer.OTB.Enums
{
    public enum NodeType : byte
    {
        NotSetYet = 0,          // Added on C# version
        RootVersion1 = 1,       // OTBM_ROOTV1
        MapData = 2,          // OTBM_MAP_DATA
        ItemDefinition = 3,     // OTBM_ITEM_DEF
        TileArea = 4,           // OTBM_TILE_AREA
        NormalTile = 5,         // OTBM_TILE
        Item = 6,               // OTBM_ITEM
        TileSquare = 7,         // OTBM_TILE_SQUARE
        TileReference = 8,      // OTBM_TILE_REF
        Spawn = 9,              // OTBM_SPAWNS
        SpawnArea = 10,         // OTBM_SPAWN_AREA
        Monster = 11,           // OTBM_MONSTER
        TownCollection = 12,    // OTBM_TOWNS
        Town = 13,              // OTBM_TOWN
        HouseTile = 14,         // OTBM_HOUSETILE
        WayPointCollection = 15,// OTBM_WAYPOINTS
        WayPoint = 16           // OTBM_WAYPOINT
    }
}
