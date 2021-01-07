using NeoServer.Game.Common;
using NeoServer.Game.Common.Location.Structs;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items
{
    public delegate void CreateItem(IItem item);
    public interface IItemFactory: IFactory
    {
        IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes);
        IItem Create(string name, Location location, IDictionary<ItemAttribute, IConvertible> attributes);
    }
}
