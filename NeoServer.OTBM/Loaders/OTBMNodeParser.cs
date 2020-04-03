using System;
using System.Linq;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Structure;

namespace NeoServer.OTBM
{
    public class OTBMNodeParser
    {
        private readonly Structure.OTBM otbm;

        public OTBMNodeParser()
        {
            otbm = new Structure.OTBM();
        }
        public Structure.OTBM Parse(OTBNode node)
        {

            otbm.Header = new Header(node);

            var worldData = GetWorldData(node);

            otbm.TileAreas = worldData.Children.Where(c => c.Type == NodeType.TileArea)
                                               .Select(c => new TileArea(c));

            otbm.Towns = worldData.Children.Where(c => c.Type == NodeType.TownCollection)
                                           .SelectMany(c => c.Children)
                                           .Select(c => new TownNode(c));

            otbm.Waypoints = worldData.Children
                                      .Where(c => c.Type == NodeType.WayPointCollection && otbm.Header.Version > 1)
                                      .SelectMany(c => c.Children)
                                      .Select(c => new WaypointNode(c));

            return otbm;
        }



        public OTBNode GetWorldData(OTBNode node)
        {
            var mapData = node.Children.SingleOrDefault();

            if (mapData == null || mapData.Type != NodeType.WorldData)
            {
                throw new Exception("OTBM_MAP_DATA is invalid");
            }

            return mapData;
        }
    }
}