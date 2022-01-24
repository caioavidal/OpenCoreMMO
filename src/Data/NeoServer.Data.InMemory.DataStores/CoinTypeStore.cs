using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Data.InMemory.DataStores;

public class CoinTypeStore : DataStore<CoinTypeStore, ushort, IItemType>, ICoinTypeStore
{
}