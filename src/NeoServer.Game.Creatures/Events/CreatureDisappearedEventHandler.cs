using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Creatures.Events
{
    public class PlayerDisappearedEventHandler : IGameEventHandler
    {
        private readonly IMap map;

        public PlayerDisappearedEventHandler(IMap map)
        {
            this.map = map;
        }

        public void Execute(IPlayer player)
        {
            foreach (var spectator in map.GetCreaturesAtPositionZone(player.Location, player.Location))
            {
                if (spectator is not IMonster monster) continue;

                if (monster.IsDead) continue;

                monster.SetAsEnemy(player);
            }
        }
    }
}
