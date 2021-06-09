using NeoServer.Game.Contracts.Chats;

namespace NeoServer.Game.Contracts.Items
{
    public interface IChatChannelEventSubscriber
    {
        public void Subscribe(IChatChannel chatChannel);
        public void Unsubscribe(IChatChannel chatChannel);
    }
}