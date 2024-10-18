using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;

public class ItemTypeStore : DataStore<ItemTypeStore, ushort, IItemType>, IItemTypeStore
{
}