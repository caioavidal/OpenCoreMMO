using System.Collections.Generic;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Data.InMemory.DataStores;

public class PlayerOutFitStore : DataStore<PlayerOutFitStore, Gender, IEnumerable<IPlayerOutFit>>, IPlayerOutFitStore
{
}