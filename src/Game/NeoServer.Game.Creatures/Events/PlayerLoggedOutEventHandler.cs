using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.DataStore;
using NeoServer.Server.Model.Players.Contracts;
using System.Linq;

namespace NeoServer.Game.Creatures.Events
{
    public class PlayerLoggedOutEventHandler : IGameEventHandler
    {
        public PlayerLoggedOutEventHandler()
        {
        }

        public void Execute(IPlayer player)
        {
            foreach (var channel in ChatChannelStore.Data.All.Where(x=>x.HasUser(player)))
            {
                player.ExitChannel(channel);
            }
        }
    }
}
