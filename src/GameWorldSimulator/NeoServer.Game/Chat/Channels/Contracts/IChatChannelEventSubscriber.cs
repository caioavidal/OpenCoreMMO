using NeoServer.Game.Chat.Channels.Contracts;

namespace NeoServer.Game.Common.Contracts.Chats;

public interface IChatChannelEventSubscriber
{
    public void Subscribe(IChatChannel chatChannel);
    public void Unsubscribe(IChatChannel chatChannel);
}