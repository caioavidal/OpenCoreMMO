using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Contracts;
using NeoServer.Server.Tasks;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureDefenseJob
    {
        public static void Execute(IMonster monster, IGameServer game)
        {
            if (monster.IsDead) return;

            if (monster.IsInCombat && !monster.Defending)
            {
                var interval = monster.Defend();

                ScheduleDefense(game, monster, interval);
            }
        }

        private static void ScheduleDefense(IGameServer game, IMonster monster, ushort interval)
        {
            if (monster.Defending)
                game.Scheduler.AddEvent(new SchedulerEvent(interval, () =>
                {
                    var interval = monster.Defend();
                    ScheduleDefense(game, monster, interval);
                }));
        }
    }
}