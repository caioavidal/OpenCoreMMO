using NeoServer.Game.Enums;
using NeoServer.Game.Enums.Location.Structs;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Contracts.Items
{
    public delegate void CreateItem(IItem item);
    public interface IItemFactory: IFactory
    {
        IItem Create(ushort typeId, Location location, IDictionary<ItemAttribute, IConvertible> attributes);
    }
}
