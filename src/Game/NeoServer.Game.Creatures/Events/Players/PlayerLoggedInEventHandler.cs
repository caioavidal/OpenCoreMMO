using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.Creatures.Events.Players
{
    public class PlayerLoggedInEventHandler : IGameEventHandler
    {
        private readonly IChatChannelStore _chatChannelStore;

        public PlayerLoggedInEventHandler(IChatChannelStore chatChannelStore)
        {
            _chatChannelStore = chatChannelStore;
        }
        public void Execute(IPlayer player)
        {
            if (player is null) return;

            var channels = _chatChannelStore.All.Where(x => x.Opened);
            channels = player.PersonalChannels is null
                ? channels
                : channels.Concat(player.PersonalChannels?.Where(x => x.Opened));
            channels = player.PrivateChannels is null
                ? channels
                : channels.Concat(player.PrivateChannels?.Where(x => x.Opened));

            foreach (var channel in channels) player.JoinChannel(channel);
        }
    }
}