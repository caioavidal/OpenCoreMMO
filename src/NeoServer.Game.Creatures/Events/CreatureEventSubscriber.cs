using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
    {
        private readonly CreatureKilledEventHandler creatureKilledEventHandler;
        private readonly CreatureDamagedEventHandler creatureDamagedEventHandler;
        private readonly CreaturePropagatedAttackEventHandler creaturePropagatedAttackEventHandler;
        private readonly CreatureTeleportedEventHandler creatureTeleportedEventHandler;

        public CreatureEventSubscriber(CreatureKilledEventHandler creatureKilledEventHandler,
            CreatureDamagedEventHandler creatureDamagedEventHandler, CreaturePropagatedAttackEventHandler creaturePropagatedAttackEventHandler, 
            CreatureTeleportedEventHandler creatureTeleportedEventHandler)
        {
            this.creatureKilledEventHandler = creatureKilledEventHandler;
            this.creatureDamagedEventHandler = creatureDamagedEventHandler;
            this.creaturePropagatedAttackEventHandler = creaturePropagatedAttackEventHandler;
            this.creatureTeleportedEventHandler = creatureTeleportedEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if (creature is ICombatActor combatActor)
            {
                combatActor.OnKilled += creatureKilledEventHandler.Execute;
                combatActor.OnDamaged += creatureDamagedEventHandler.Execute;
                combatActor.OnPropagateAttack += creaturePropagatedAttackEventHandler.Execute;
            }
            if(creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnTeleported += creatureTeleportedEventHandler.Execute;
            }
        }

        public void Unsubscribe(ICreature creature)
        {
            if (creature is ICombatActor combatActor)
            {
                combatActor.OnKilled -= creatureKilledEventHandler.Execute;
                combatActor.OnDamaged -= creatureDamagedEventHandler.Execute;
                combatActor.OnPropagateAttack -= creaturePropagatedAttackEventHandler.Execute;
            }
            if (creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnTeleported -= creatureTeleportedEventHandler.Execute;
            }
        }
    }
}
