using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IActionStore: IDataStore<ushort,ItemAction>
{
}