using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace NeoServer.Server.Loaders.Tests
{
    public class ItemLoaderShould
    {
        [Fact]
        public void LoadItems()
        {
            var sup = new ItemLoader();

            var a = sup.LoadItems();
        }
    }
}
