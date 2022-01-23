using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Data.InMemory.DataStores;

public class ChatChannelStore : DataStore<ChatChannelStore, ushort, IChatChannel>, IChatChannelStore
{
}