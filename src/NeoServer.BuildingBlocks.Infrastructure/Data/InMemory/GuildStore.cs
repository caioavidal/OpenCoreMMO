using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;

public class GuildStore : DataStore<GuildStore, ushort, IGuild>, IGuildStore
{
}