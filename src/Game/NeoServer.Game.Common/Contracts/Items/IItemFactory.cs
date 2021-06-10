using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.Items
{
    public delegate void CreateItem(IItem item);

    public interface IItemFactory : IFactory
    {
        IItem Create(ushort typeId, Location.Structs.Location location, IDictionary<ItemAttribute, IConvertible> attributes);
        IItem Create(string name, Location.Structs.Location location, IDictionary<ItemAttribute, IConvertible> attributes);
        IEnumerable<ICoin> CreateCoins(ulong amount);
        IItem CreateLootCorpse(ushort typeId, Location.Structs.Location location, ILoot loot);
    }
}