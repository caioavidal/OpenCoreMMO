using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using Xunit;

namespace NeoServer.OTB.Tests.Parsers
{
    public class OTBBinaryTreeBuilderTest
    {
        [Fact]
        public void Deserialize_ReturnsOTBNodeInstance()
        {
            var data = new byte[]{
                0,0,0,0,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.MapData,
                3,
                (byte)OTBMarkupByte.Escape,
                (byte)OTBMarkupByte.Escape,
                4,
                6,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.TileArea,
                3,
                4,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.NormalTile,
                2,

                (byte)OTBMarkupByte.End,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.NormalTile,
                7,

                (byte)OTBMarkupByte.Start,
                (byte) NodeType.Item,
                6,
                (byte)OTBMarkupByte.End,

                (byte)OTBMarkupByte.Start,
                (byte) NodeType.Item,
                9,
                5,

                (byte)OTBMarkupByte.Start,
                (byte) NodeType.Item,
                3,
                (byte)OTBMarkupByte.End,

                (byte)OTBMarkupByte.End,

                (byte)OTBMarkupByte.End,
                (byte)OTBMarkupByte.End,
                (byte)OTBMarkupByte.Start,
                 (byte) NodeType.TileArea,
                3,
                4,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.NormalTile,
                2,

                (byte)OTBMarkupByte.End,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.NormalTile,
                7,

                (byte)OTBMarkupByte.End,
                (byte)OTBMarkupByte.End,
                (byte)OTBMarkupByte.End
            };

            var node = OTBBinaryTreeBuilder.Deserialize(data);

            Assert.NotNull(node);//todo
        }
    }
}
