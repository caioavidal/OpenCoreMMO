using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Creatures.Players;

namespace NeoServer.Infrastructure.InMemory;

public class PlayerOutFitStore : DataStore<PlayerOutFitStore, Gender, IEnumerable<IPlayerOutFit>>, IPlayerOutFitStore
{
}