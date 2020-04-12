using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class GameCreatureJob
    {
        private const ushort EVENT_CHECK_CREATURE_INTERVAL = 100;
        private readonly Game game;

        public GameCreatureJob(Game game)
        {
            this.game = game;
        }



        public void StartCheckingCreatures()
        {
            game.Scheduler.AddEvent(new SchedulerEvent(EVENT_CHECK_CREATURE_INTERVAL, StartCheckingCreatures));


            foreach (var creature in game.CreatureManager.GetCreatures())
            {
                if (creature.IsDead)
                {
                    continue;
                }
                if (creature is IPlayer)
                {
                    PlayerPingJob.Execute((IPlayer)creature, game);
                }

            }

        }
    }
}
