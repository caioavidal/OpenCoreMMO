using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Infrastructure.InMemory;

public class CoinTypeStore : DataStore<CoinTypeStore, ushort, IItemType>, ICoinTypeStore
{
}