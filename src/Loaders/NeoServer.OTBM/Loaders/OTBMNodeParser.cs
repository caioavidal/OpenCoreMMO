using System;
using System.Linq;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Structure;
using NeoServer.OTBM.Structure.TileArea;
using NeoServer.OTBM.Structure.Towns;

namespace NeoServer.OTBM.Loaders;

/// <summary>
///     A class to parse <see cref="OtbNode"></see> structure to <see cref="Otbm" /> instance
/// </summary>
public sealed class OTBMNodeParser
{
    private readonly Otbm otbm;

    public OTBMNodeParser()
    {
        otbm = new Otbm();
    }

    /// <summary>
    ///     Parses the OTBNode binary tree structure to a OTBM instance <see cref="Otbm.Structure.OTBM"></see>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Otbm Parse(OtbNode node)
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

    private MapData GetMapData(OtbNode mapData)
    {
        if (mapData.Type != NodeType.MapData) throw new Exception("Could not read root data node");

        return new MapData(mapData);
    }
}