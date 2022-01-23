using NeoServer.Game.Common.Contracts.Chats;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IChatChannelStore : IDataStore<ushort, IChatChannel>
{
}