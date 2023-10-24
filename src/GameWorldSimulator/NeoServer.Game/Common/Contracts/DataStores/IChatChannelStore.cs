using NeoServer.Game.Chat.Channels.Contracts;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IChatChannelStore : IDataStore<ushort, IChatChannel>
{
}