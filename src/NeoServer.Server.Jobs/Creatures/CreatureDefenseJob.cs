using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureDefenseJob
    {
        private const uint INTERVAL = 1000;
        public static void Execute(ICreature creature, Game game)
        {
            if (!(creature is IMonster monster))
            {
                return;
            }
            if (monster.IsDead)
            {
                return;
            }
            if (monster.IsInCombat && !monster.Defending)
            {
                var interval = monster.Defende();

                ScheduleDefense(game, monster, interval);
            }
        }

        private static void ScheduleDefense(Game game, IMonster monster, ushort interval)
        {

            if (monster.Defending)
            {
                game.Scheduler.AddEvent(new SchedulerEvent(interval, () =>
                {
                    var interval = monster.Defende();
                    ScheduleDefense(game, monster, interval);
                }));
            }
        }
    }
}
