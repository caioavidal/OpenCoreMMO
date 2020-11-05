using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Effects.Explosion;
using NeoServer.Game.Effects.Magical;
using NeoServer.Game.Enums.Location.Structs;
using NeoServer.Game.Parsers.Effects;
using NeoServer.Networking.Packets.Outgoing;
using NeoServer.Server.Contracts.Network;
using NeoServer.Server.Tasks;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NeoServer.Server.Events.Combat
{
    public class CreatureChangedAttackTargetEventHandler
    {
        private readonly Game game;

        public CreatureChangedAttackTargetEventHandler(Game game)
        {
            this.game = game;
        }
        public void Execute(ICombatActor actor, uint oldTarget, uint newTarget)
        {
            if (actor.AttackEvent != 0)
            {
                return;
            }

            Attack(actor);

            actor.AttackEvent = game.Scheduler.AddEvent(new SchedulerEvent((int)actor.BaseAttackSpeed, () => Attack(actor)));
        }

        private void Attack(ICombatActor actor)
        {
            if (actor.Attacking)
            {
                if (game.CreatureManager.TryGetCreature(actor.AutoAttackTargetId, out var creature) && creature is ICombatActor enemy)
                {
                    actor.Attack(enemy, null);
                    MoveAroundEnemy(actor);
                }
            }
            else
            {
                if (actor.AttackEvent != 0)
                {
                    game.Scheduler.CancelEvent(actor.AttackEvent);
                    actor.AttackEvent = 0;
                }
            }

            if (actor.AttackEvent != 0)
            {
                actor.AttackEvent = 0;
                Execute(actor, 0, 0);
            }
        }

        private void MoveAroundEnemy(ICombatActor actor)
        {
            if (!(actor is IMonster monster)) return;

            monster.MoveAroundEnemy();
        }
    }
}