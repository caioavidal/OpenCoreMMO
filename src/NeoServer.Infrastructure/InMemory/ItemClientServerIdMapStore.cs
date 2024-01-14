using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Infrastructure.InMemory;

public class ItemClientServerIdMapStore : DataStore<ItemClientServerIdMapStore, ushort, ushort>,
    IItemClientServerIdMapStore
{
}