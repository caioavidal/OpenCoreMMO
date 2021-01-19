using NeoServer.OTB.Enums;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Structure;
using System;
using System.Linq;

namespace NeoServer.OTBM
{

    /// <summary>
    /// A class to parse <see cref="OTBNode"></see> structure to <see cref="Structure.OTBM"/> instance
    /// </summary>
    public sealed class OTBMNodeParser
    {
        private readonly Structure.OTBM otbm;

        public OTBMNodeParser()
        {
            otbm = new Structure.OTBM();
        }

        /// <summary>
        /// Parses the OTBNode binary tree structure to a OTBM instance <see cref="OTBM.Structure.OTBM"></see>
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Structure.OTBM Parse(OTBNode node)
        {

            otbm.Header = new Header(node);

            var mapData = node.Children.SingleOrDefault();

            otbm.MapData = GetMapData(mapData);

            otbm.TileAreas = mapData.Children.Where(c => c.Type == NodeType.TileArea)
                                               .Select(c => new TileArea(c));

            otbm.Towns = mapData.Children.Where(c => c.Type == NodeType.TownCollection)
                                           .SelectMany(c => c.Children)
                                           .Select(c => new TownNode(c));

            otbm.Waypoints = mapData.Children
                                      .Where(c => c.Type == NodeType.WayPointCollection && otbm.Header.Version > 1)
                                      .SelectMany(c => c.Children)
                                      .Select(c => new WaypointNode(c));

            return otbm;
        }

        private MapData GetMapData(OTBNode mapData)
        {
            if (mapData.Type != NodeType.MapData)
            {
                throw new Exception("Could not read root data node");
            }

            return new MapData(mapData);
        }
    }
}