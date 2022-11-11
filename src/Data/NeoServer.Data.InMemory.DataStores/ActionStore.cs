using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Item;

namespace NeoServer.Data.InMemory.DataStores;
public class ActionStore: DataStore<ActionStore, ushort, ItemAction>, IActionStore
{
}