using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;

public class ItemClientServerIdMapStore : DataStore<ItemClientServerIdMapStore, ushort, ushort>,
    IItemClientServerIdMapStore
{
}