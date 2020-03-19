using NeoServer.Server.Loaders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Server.Items.Tests
{
    public class ItemLoaderShould
    {
        [Fact]
        public void LoadItems()
        {
            ItemLoader.Load();

            var result = ItemData.Items;

            Assert.Equal(12561, result.Count);
        }
    }
}
