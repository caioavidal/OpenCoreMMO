using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface ICoinTypeStore : IDataStore<ushort, IItemType>
{
}