using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.DataStore
{
    public class ChatChannelStore : DataStore<ChatChannelStore, ushort, IChatChannel>, IChatChannelStore
    {
    }
}