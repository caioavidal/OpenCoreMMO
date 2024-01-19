using System;
using NeoServer.Application.Features.Chat.Channel.SendMessageToChannel;
using NeoServer.Game.Chat.Channels.Contracts;

namespace NeoServer.Server.Events.Subscribers;

public class ChatChannelEventSubscriber : IChatChannelEventSubscriber
{
    private readonly ChatMessageAddedEventHandler chatMessageAddedEventHandler;

    public ChatChannelEventSubscriber(ChatMessageAddedEventHandler chatMessageAddedEventHandler)
    {
        this.chatMessageAddedEventHandler = chatMessageAddedEventHandler;
    }

    public void Subscribe(IChatChannel chatChannel)
    {
        chatChannel.OnMessageAdded += chatMessageAddedEventHandler.Execute;
    }

    public void Unsubscribe(IChatChannel chatChannel)
    {
        throw new NotImplementedException();
    }
}