using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Data.InMemory.DataStores;

public class GuildStore : DataStore<GuildStore, ushort, IGuild>, IGuildStore
{
}