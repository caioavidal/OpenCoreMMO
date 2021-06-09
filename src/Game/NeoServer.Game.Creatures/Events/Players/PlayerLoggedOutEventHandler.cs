using System.Linq;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.DataStore;

namespace NeoServer.Game.Creatures.Events
{
    public class PlayerLoggedOutEventHandler : IGameEventHandler
    {
        public void Execute(IPlayer player)
        {
            ExitChannels(player);
        }

        private void ExitChannels(IPlayer player)
        {
            foreach (var channel in ChatChannelStore.Data.All.Where(x => x.HasUser(player)))
                player.ExitChannel(channel);

            if (player.PersonalChannels is not null)
                foreach (var channel in player.PersonalChannels)
                    player.ExitChannel(channel);
            if (player.PrivateChannels is not null)
                foreach (var channel in player.PrivateChannels)
                    player.ExitChannel(channel);
        }
    }
}