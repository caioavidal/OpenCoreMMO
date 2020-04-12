using NeoServer.OTB.DataStructures;
using System;
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


    }
}
