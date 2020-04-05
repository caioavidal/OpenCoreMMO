using System;
using System.Collections.Generic;
using NeoServer.OTB.DataStructures;
using NeoServer.OTB.Enums;
using NeoServer.OTB.Parsers;
using Xunit;

namespace NeoServer.OTB.Tests
{
    public class ReadOnlyArrayTest
    {



        [Fact]
        public void WrapCollection_NullItems_Throws()
        {
            Assert.Throws<NullReferenceException>(() => ReadOnlyArray<int>.WrapCollection(null));
        }

        [Fact]
        public void WrapCollection_NotNull_ReturnsInstance()
        {
            var instance = ReadOnlyArray<int>.WrapCollection(new int[0]);
            Assert.NotNull(instance);
            Assert.IsType<ReadOnlyArray<int>>(instance);
        }

        [Fact]
        public void GetByIndex_ReturnsValue()
        {
            var instance = ReadOnlyArray<int>.WrapCollection(new int[4] { 4, 7, 5, 1 });

            Assert.Equal(5, instance[2]);
            Assert.Equal(7, instance[1]);
            Assert.Equal(4, instance[0]);
            Assert.Throws<IndexOutOfRangeException>(() => instance[8]);
            Assert.Throws<IndexOutOfRangeException>(() => instance[-5]);
        }
        [Fact]
        public void GetCount_ReturnsArrayLength()
        {
            var instance = ReadOnlyArray<int>.WrapCollection(new int[4] { 4, 7, 5, 1 });

            Assert.Equal(4, instance.Count);

        }

        [Fact]
        public void GetEnumerator_ReturnsGenericEnumerator()
        {
            var data = new int[4] { 4, 7, 5, 1 };
            var instance = ReadOnlyArray<int>.WrapCollection(data);

            Assert.NotNull(instance.GetEnumerator());
        }

        [Fact]
        public void Test()
        {
            var data = new byte[]{
                0,0,0,0,
                (byte)OTBMarkupByte.Start,
                (byte) NodeType.WorldData,
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

            //Assert.NotNull(instance.GetEnumerator());
        }
    }
}
