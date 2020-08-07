using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Enums;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Model.Players.Contracts;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Server.Commands.Combat
{
    public class CreatureAttackCommand //: Command
    {

        public static void Execute(IPlayer player, uint targetId, Game game, IConnection connection)
        {
            if (targetId == 0)
            {
                player.SetAttackTarget(0);
                return;
            }

            if (!game.CreatureManager.TryGetCreature(targetId, out ICreature creature))
            {
                return;
            }

         

          

            var remainingCooldown = player.CalculateRemainingCooldownTime(CooldownType.Combat, DateTime.Now);

           // Console.WriteLine($"remainingCooldown.Milliseconds {remainingCooldown}");

            if (remainingCooldown > 0 && targetId == player.AutoAttackTargetId)
            {
                return;
            }

            if(targetId != player.AutoAttackTargetId)
            {
                player.SetAttackTarget(targetId);
            }

            game.Scheduler.CancelEvent(player.LastCombatEvent);
            game.Scheduler.AddEvent(new SchedulerEvent((int)remainingCooldown, () => StartAutoAttack(player, creature, game)));
        }

        public static void StartAutoAttack(IPlayer player, ICreature creature, Game game)
        {   
            if (player.AutoAttackTargetId > 0)
            {
                player.Attack(creature);

                var remainingCooldown = player.CalculateRemainingCooldownTime(CooldownType.Combat, DateTime.Now);
                player.LastCombatEvent = game.Scheduler.AddEvent(new SchedulerEvent((int)remainingCooldown, () => StartAutoAttack(player, creature, game)));

            }
        }
    }
}
