using System;
using System.Linq;
using NeoServer.Loaders.OTB.Enums;
using NeoServer.Loaders.OTB.Structure;
using NeoServer.Loaders.OTBM.Structure;
using NeoServer.Loaders.OTBM.Structure.TileArea;
using NeoServer.Loaders.OTBM.Structure.Towns;

namespace NeoServer.Loaders.OTBM.Loaders;

/// <summary>
///     A class to parse <see cref="OtbNode"></see> structure to <see cref="Otbm" /> instance
/// </summary>
public sealed class OTBMNodeParser
{
    private readonly Otbm _otbm;

    public OTBMNodeParser()
    {
        _otbm = new Otbm();
    }

    /// <summary>
    ///     Parses the OTBNode binary tree structure to a OTBM instance <see cref="Otbm.Structure.OTBM"></see>
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public Otbm Parse(OtbNode node)
    {
        _otbm.Header = new Header(node);

        var mapData = node.Children.SingleOrDefault();

        if (mapData is null) return _otbm;

        _otbm.MapData = GetMapData(mapData);

        _otbm.TileAreas = mapData.Children.Where(c => c.Type == NodeType.TileArea)
            .Select(c => new TileArea(c));

        _otbm.Towns = mapData.Children.Where(c => c.Type == NodeType.TownCollection)
            .SelectMany(c => c.Children)
            .Select(c => new TownNode(c));

        _otbm.Waypoints = mapData.Children
            .Where(c => c.Type == NodeType.WayPointCollection && _otbm.Header.Version > 1)
            .SelectMany(c => c.Children)
            .Select(c => new WaypointNode(c));

        return _otbm;
    }

    private static MapData GetMapData(OtbNode mapData)
    {
        if (mapData.Type != NodeType.MapData) throw new Exception("Could not read root data node");

        return new MapData(mapData);
    }
}