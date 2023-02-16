using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Game.Creatures.Events;

public class PlayerDisappearedEventHandler : IGameEventHandler
{
    private readonly IMap map;

    public PlayerDisappearedEventHandler(IMap map)
    {
        this.map = map;
    }

    public void Execute(IPlayer player)
    {
        //foreach (var spectator in map.GetCreaturesAtPositionZone(player.Location, player.Location))
        //{
        //    if (spectator is not IMonster monster) continue;

        //    if (monster.IsDead) continue;

        //    monster.SetAsEnemy(player);
        //}
    }
}