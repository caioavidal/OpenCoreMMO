using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Server.Commands.Movement
{
    public class WalkToMechanism
    {
        private Action<ICreature> callBack;

        public void DoOperation(IPlayer player, Action action, Location toLocation, Game game, bool secondChance = false)
        {
            if (secondChance)
            {
                player.OnCompleteWalking -= callBack.Invoke;
            }

            if (!toLocation.IsNextTo(player.Location))
            {
                if (secondChance) return;

                callBack = (creature) => game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelayMilliseconds, () => DoOperation(player, action, toLocation, game, true)));

                player.WalkTo(toLocation, callBack);
                return;
            }

            action.Invoke();
        }
    }
}
