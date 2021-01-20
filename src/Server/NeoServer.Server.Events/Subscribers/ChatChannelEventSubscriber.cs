using NeoServer.Game.Contracts.Chats;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Useables;
using NeoServer.Server.Events.Chat;

namespace NeoServer.Server.Events
{
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
            throw new System.NotImplementedException();
        }
    }
}