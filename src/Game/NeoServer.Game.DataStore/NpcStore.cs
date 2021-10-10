using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.DataStore
{
    public class NpcStore : DataStore<NpcStore, string, INpcType>, INpcStore
    {
    }
}