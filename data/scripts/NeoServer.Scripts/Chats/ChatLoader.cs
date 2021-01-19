using NeoServer.Game.Chats;
using NeoServer.Game.DataStore;
using NeoServer.Loaders.Interfaces;

namespace NeoServer.Scripts.Chats
{
    public class ChatLoader : ICustomLoader
    {
        public void Load()
        {
            ChatChannelStore.Data.Add(1, new ChatChannel(1, "channel1", 1));
            ChatChannelStore.Data.Add(2, new ChatChannel(1, "channel2", 1));
            ChatChannelStore.Data.Add(3, new ChatChannel(1, "channel3", 1));
        }
    }
}
