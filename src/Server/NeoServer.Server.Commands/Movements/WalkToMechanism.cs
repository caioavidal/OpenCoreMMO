using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Tasks;
using System;

namespace NeoServer.Server.Commands.Movement
{
    public class WalkToMechanism
    {
        public static void DoOperation(IPlayer player, Action action, Location toLocation, IGameServer game, bool secondChance = false)
        {
            if (!toLocation.IsNextTo(player.Location))
            {
                if (secondChance) return;

                Action<ICreature> callBack = (creature) => game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelay, () => DoOperation(player, action, toLocation, game, true)));

                player.WalkTo(toLocation, callBack);
                return;
            }

            action?.Invoke();
        }
    }
}
