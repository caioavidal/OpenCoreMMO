using NeoServer.OTB.Parsers;
using NeoServer.OTB.Structure;
using NeoServer.OTBM.Enums;

namespace NeoServer.OTBM.Structure
{
    /// <summary>
    /// Contains all the Map metadata 
    /// </summary>
    public class MapData
    {
        /// <summary>
        /// Map description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Spawn file name
        /// </summary>
        public string SpawnFile { get; set; }
        /// <summary>
        /// House file name
        /// </summary>
        public string HouseFile { get; set; }

        public MapData(OTBNode node)
        {
            var stream = new OTBParsingStream(node.Data);

            while (!stream.IsOver)
            {
                var attribute = (NodeAttribute)stream.ReadByte();
                var value = stream.ReadString();

                ParseAttribute(attribute, value);
            }

        }

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
                default:
                    break;
                    //throw new ArgumentException($"invalid attribute {attribute}");
            }
        }
    }
}