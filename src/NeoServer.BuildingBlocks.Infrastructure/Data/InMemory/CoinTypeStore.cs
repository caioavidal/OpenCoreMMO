using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;

public class CoinTypeStore : DataStore<CoinTypeStore, ushort, IItemType>, ICoinTypeStore
{
}