using NeoServer.Game.Common.Contracts.Creatures;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IGuildStore : IDataStore<ushort, IGuild>
{
}