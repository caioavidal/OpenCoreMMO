using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Server.Common.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Commands.Movements;

public class WalkToMechanism : IWalkToMechanism
{
    private readonly IGameServer game;

    public WalkToMechanism(IGameServer game)
    {
        this.game = game;
    }

    public void WalkTo(IPlayer player, Action action, Location toLocation, bool secondChance = false)
    {
        if (!toLocation.IsNextTo(player.Location))
        {
            if (secondChance) return;

            Action<ICreature> callBack = _ =>
                game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelay,
                    () => WalkTo(player, action, toLocation, true)));

            player.WalkTo(toLocation, callBack);
            return;
        }

        action?.Invoke();
    }
}