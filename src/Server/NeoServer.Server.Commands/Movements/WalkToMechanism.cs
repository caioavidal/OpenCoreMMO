using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Tasks;
using System;

namespace NeoServer.Server.Commands.Movement
{
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

                Action<ICreature> callBack = (creature) => game.Scheduler.AddEvent(new SchedulerEvent(player.StepDelay, () => WalkTo(player, action, toLocation, true)));

                player.WalkTo(toLocation, callBack);
                return;
            }

            action?.Invoke();
        }
    }
}
