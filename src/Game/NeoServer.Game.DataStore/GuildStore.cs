using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.DataStore
{
    public class GuildStore : DataStore<GuildStore, ushort, IGuild>, IGuildStore
    {
    }
}