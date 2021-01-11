using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Server.Model.Players.Contracts;

namespace NeoServer.Game.Creatures.Events
{
    public class CreatureEventSubscriber : ICreatureEventSubscriber, IGameEventSubscriber
    {
        private readonly CreatureKilledEventHandler creatureKilledEventHandler;
        private readonly CreatureDamagedEventHandler creatureDamagedEventHandler;
        private readonly CreaturePropagatedAttackEventHandler creaturePropagatedAttackEventHandler;
        private readonly CreatureTeleportedEventHandler creatureTeleportedEventHandler;
        private readonly PlayerDisappearedEventHandler playerDisappearedEventHandler;
        private readonly CreatureMovedEventHandler creatureMovedEventHandler;

        public CreatureEventSubscriber(CreatureKilledEventHandler creatureKilledEventHandler,
            CreatureDamagedEventHandler creatureDamagedEventHandler, CreaturePropagatedAttackEventHandler creaturePropagatedAttackEventHandler,
            CreatureTeleportedEventHandler creatureTeleportedEventHandler, PlayerDisappearedEventHandler playerDisappearedEventHandler, 
            CreatureMovedEventHandler creatureMovedEventHandler)
        {
            this.creatureKilledEventHandler = creatureKilledEventHandler;
            this.creatureDamagedEventHandler = creatureDamagedEventHandler;
            this.creaturePropagatedAttackEventHandler = creaturePropagatedAttackEventHandler;
            this.creatureTeleportedEventHandler = creatureTeleportedEventHandler;
            this.playerDisappearedEventHandler = playerDisappearedEventHandler;
            this.creatureMovedEventHandler = creatureMovedEventHandler;
        }

        public void Subscribe(ICreature creature)
        {
            if (creature is ICombatActor combatActor)
            {
                combatActor.OnKilled += creatureKilledEventHandler.Execute;
                combatActor.OnDamaged += creatureDamagedEventHandler.Execute;
                combatActor.OnPropagateAttack += creaturePropagatedAttackEventHandler.Execute;
            }
            if (creature is IWalkableCreature walkableCreature)
            {
                walkableCreature.OnTeleported += creatureTeleportedEventHandler.Execute;
                walkableCreature.OnCreatureMoved += creatureMovedEventHandler.Execute;
            }
            if (creature is IPlayer player)
            {
                player.OnLoggedOut += playerDisappearedEventHandler.Execute;
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
                walkableCreature.OnCreatureMoved -= creatureMovedEventHandler.Execute;

            }
            if (creature is IPlayer player)
            {
                player.OnLoggedOut -= playerDisappearedEventHandler.Execute;
            }
        }
    }
}
