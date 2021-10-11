using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.DataStores;

namespace NeoServer.Game.Creatures.Events.Players
{
    public class PlayerLoggedOutEventHandler : IGameEventHandler
    {
        private readonly IChatChannelStore _chatChannelStore;

        public PlayerLoggedOutEventHandler(IChatChannelStore chatChannelStore)
        {
            _chatChannelStore = chatChannelStore;
        }
        public void Execute(IPlayer player)
        {
            ExitChannels(player);
        }

        private void ExitChannels(IPlayer player)
        {
            foreach (var channel in _chatChannelStore.All.Where(x => x.HasUser(player)))
                player.ExitChannel(channel);

            if (player.PersonalChannels is not null)
                foreach (var channel in player.PersonalChannels)
                    player.ExitChannel(channel);
            if (player.PrivateChannels is null) return;
            {
                foreach (var channel in player.PrivateChannels)
                    player.ExitChannel(channel);
            }
        }
    }
}