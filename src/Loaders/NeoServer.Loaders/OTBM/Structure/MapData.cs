using NeoServer.Loaders.OTB.Parsers;
using NeoServer.Loaders.OTB.Structure;
using NeoServer.Loaders.OTBM.Enums;

namespace NeoServer.Loaders.OTBM.Structure;

/// <summary>
///     Contains all the Map metadata
/// </summary>
public class MapData
{
    public MapData(OtbNode node)
    {
        var stream = new OtbParsingStream(node.Data);

        while (!stream.IsOver)
        {
            var attribute = (NodeAttribute)stream.ReadByte();
            var value = stream.ReadString();

            ParseAttribute(attribute, value);
        }
    }

    /// <summary>
    ///     Map description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Spawn file name
    /// </summary>
    public string SpawnFile { get; set; }

    /// <summary>
    ///     House file name
    /// </summary>
    public string HouseFile { get; set; }

    private void ParseAttribute(NodeAttribute attribute, string value)
    {
        switch (attribute)
        {
            case NodeAttribute.WorldDescription:
                Description = value;
                break;
            case NodeAttribute.ExtensionFileForSpawns:
                SpawnFile = value;
                break;
            case NodeAttribute.ExtensionFileForHouses:
                HouseFile = value;
                break;
        }
    }
}