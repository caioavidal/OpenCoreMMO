using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Data.InMemory.DataStores;

public class ItemTypeStore : DataStore<ItemTypeStore, ushort, IItemType>, IItemTypeStore
{
}