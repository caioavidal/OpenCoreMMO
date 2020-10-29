using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures;

namespace NeoServer.Server.Jobs.Creatures
{
    public class CreatureAttakingJob
    {
        private const uint INTERVAL = 1000;
        public static void Execute(ICombatActor creature, Game game)
        {
            //if (creature.IsDead)
            //{
            //    return;
            //}
            //if (creature.Attacking)
            //{
            //    if (!game.CreatureManager.TryGetCreature(creature.AutoAttackTargetId, out ICreature enemy))
            //    {
            //        return;
            //    }

            //    creature.Attack(enemy);
            //}
        }
    }
}
