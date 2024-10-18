using NeoServer.BuildingBlocks.Infrastructure.Data.InMemory;
using NeoServer.Game.Chat.Channels.Contracts;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Modules.Chat.Channel;

public class ChatChannelStore : DataStore<ChatChannelStore, ushort, IChatChannel>, IChatChannelStore
{
}