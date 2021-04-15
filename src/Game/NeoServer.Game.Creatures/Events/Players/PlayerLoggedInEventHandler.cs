using NeoServer.Game.Contracts;
using NeoServer.Game.DataStore;
using NeoServer.Game.Contracts.Creatures;
using System.Linq;

namespace NeoServer.Game.Creatures.Events
{
    public class PlayerLoggedInEventHandler : IGameEventHandler
    {
        public void Execute(IPlayer player)
        {
            if (player is null) return;

            var channels = ChatChannelStore.Data.All.Where(x => x.Opened);
            channels = player.PersonalChannels is null ? channels : channels.Concat(player.PersonalChannels?.Where(x => x.Opened));
            channels = player.PrivateChannels is null ? channels : channels.Concat(player.PrivateChannels?.Where(x => x.Opened));

            foreach (var channel in channels)
            {
                player.JoinChannel(channel);
            }
        }
    }
}
