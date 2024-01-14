namespace NeoServer.Game.Chat.Channels.Contracts;

public interface IChatChannelEventSubscriber
{
    public void Subscribe(IChatChannel chatChannel);
    public void Unsubscribe(IChatChannel chatChannel);
}