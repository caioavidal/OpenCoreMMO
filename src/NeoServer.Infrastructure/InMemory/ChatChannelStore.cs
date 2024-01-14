using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Infrastructure.InMemory;

public class ChatChannelStore : DataStore<ChatChannelStore, ushort, IChatChannel>, IChatChannelStore
{
}