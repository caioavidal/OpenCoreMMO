using System.Collections.Generic;

namespace NeoServer.OTBM.Structure
{
    #region OTBM Structure
    /*
	OTBM_ROOTV1
	|
	|--- OTBM_MAP_DATA
	|	|
	|	|--- OTBM_TILE_AREA
	|	|	|--- OTBM_TILE
	|	|	|--- OTBM_TILE_SQUARE (not implemented)
	|	|	|--- OTBM_TILE_REF (not implemented)
	|	|	|--- OTBM_HOUSETILE
	|	|
	|	|--- OTBM_SPAWNS (not implemented)
	|	|	|--- OTBM_SPAWN_AREA (not implemented)
	|	|	|--- OTBM_MONSTER (not implemented)
	|	|
	|	|--- OTBM_TOWNS
	|	|	|--- OTBM_TOWN
	|	|
	|	|--- OTBM_WAYPOINTS
	|		|--- OTBM_WAYPOINT
	|
	|--- OTBM_ITEM_DEF (not implemented)
*/
    #endregion

    /// <summary>
    /// OTBM class which represents the OTBM structure
    /// </summary>
    public class OTBM
    {
        /// <summary>
        /// OTBM Header data
        /// </summary>
        public Header Header { get; set; }
        /// <summary>
        /// OTBM Map metadata
        /// </summary>
        public MapData MapData { get; set; }

        /// <summary>
        /// OTBM Tile Areas
        /// </summary>
        public IEnumerable<TileArea> TileAreas { get; set; }

        /// <summary>
        /// OTBM Towns
        /// </summary>
        public IEnumerable<TownNode> Towns { get; set; }

        /// <summary>
        /// OTBM Waypoints
        /// </summary>
        public IEnumerable<WaypointNode> Waypoints { get; set; }

    }
}
