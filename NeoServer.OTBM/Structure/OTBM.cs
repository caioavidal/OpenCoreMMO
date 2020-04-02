using System;
using System.Collections.Generic;
using NeoServer.OTBM.Structure;

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
    public class OTBM
    {
        public Header Header { get; set; }

        public IEnumerable<TileArea> TileAreas { get; set; }
        // public MapType Spawns { get; set; }
        public IEnumerable<TownNode> Towns { get; set; }
        public IEnumerable<WaypointNode> Waypoints { get; set; }

    }
}
