using NeoServer.Game.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Contracts
{
    public interface IFactory
    {
        public event CreateItem OnItemCreated;
    }
}
