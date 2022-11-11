using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Data.InMemory.DataStores;

public class ActionIdMapStore: DataStore<ActionIdMapStore, ushort, List<IItem>>, IActionIdMapStore
{
}