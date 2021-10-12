using System.Collections.Generic;
using System.Linq;
using NeoServer.Game.Chats;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Chats;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.Creatures.Events.Players
{
    public class PlayerLoggedOutEventHandler : IGameEventHandler
    {
        private readonly IChatChannelStore _chatChannelStore;
        private readonly IGuildStore _guildStore;

        public PlayerLoggedOutEventHandler(IChatChannelStore chatChannelStore, IGuildStore guildStore)
        {
            _chatChannelStore = chatChannelStore;
            _guildStore = guildStore;
        }
        public void Execute(IPlayer player)
        {
            ExitChannels(player);
        }

        private void ExitChannels(IPlayer player)
        {
            foreach (var channel in _chatChannelStore.All.Where(x => x.HasUser(player)))
                player.Channel.ExitChannel(channel);

            if (player.Channel.PersonalChannels is not null)
                foreach (var channel in player.Channel.PersonalChannels)
                    player.Channel.ExitChannel(channel);
            
            if (player.Channel.PrivateChannels is not { } privateChatChannels) return;
            {
                foreach (var channel in privateChatChannels)
                    player.Channel.ExitChannel(channel);
            }
        }
    }
}