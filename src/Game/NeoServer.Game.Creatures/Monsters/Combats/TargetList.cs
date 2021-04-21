using NeoServer.Game.Combat;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Monsters.Combats
{
    public class TargetList : IEnumerable<CombatTarget>
    {
        private IDictionary<uint, CombatTarget> targets;
        private readonly IMonster monster;

        public TargetList(IMonster monster)
        {
            this.monster = monster;
        }

        public void AddTarget(ICombatActor creature)
        {
            if (targets is null) targets = new Dictionary<uint, CombatTarget>(150);

            if(!targets.TryAdd(creature.CreatureId, new CombatTarget(creature))) return;
            AttachToTargetEvents(creature);
        }
        public void RemoveTarget(ICreature creature)
        {
            if (creature is ICombatActor actor) DettachFromTargetEvents(actor);

            targets?.Remove(creature.CreatureId);

            if (monster.AutoAttackTargetId == creature.CreatureId) monster.StopAttack();
            
        }

        public void Clear()
        {
            if (targets is null) return;
            foreach (var target in targets)
            {
                RemoveTarget(target.Value.Creature);
            }
        }
        public bool Any() => targets?.Any() ?? false;
        public bool TryGetTarget(uint id, out CombatTarget target)
        {
            target = null;
            return targets?.TryGetValue(id, out target) ?? false;
        }

        #region Target Event Handlers
        private void AttachToTargetEvents(ICombatActor creature)
        {
            creature.OnKilled += OnTargetDie;
            creature.OnChangedVisibility += OnTargetDisappeared;
            creature.OnCreatureMoved += OnTargetMoved;
            if (creature is IPlayer player) player.OnLoggedOut += OnTargetRemoved;
        }
        private void DettachFromTargetEvents(ICombatActor creature)
        {
            creature.OnKilled -= OnTargetDie;
            creature.OnChangedVisibility -= OnTargetDisappeared;
            creature.OnCreatureMoved -= OnTargetMoved;
            if (creature is IPlayer player) player.OnLoggedOut -= OnTargetRemoved;
        }
        private void OnTargetDie(ICreature creature, IThing by, ILoot loot) => RemoveTarget(creature);
        private void OnTargetDisappeared(ICreature creature)
        {
            if (monster.CanSee(creature)) return;
            RemoveTarget(creature);
        }
        private void HandleTargetMoved(IWalkableCreature creature, bool lastStep = false)
        {
            if (!monster.CanSee(creature.Location))
            {
                RemoveTarget(creature);
                return;
            }
        }
        private void OnTargetMoved(IWalkableCreature creature, Location fromLocation, Location toLocation, ICylinderSpectator[] spectators)=> HandleTargetMoved(creature);        
        private void OnTargetRemoved(ICreature creature) => RemoveTarget(creature);
        #endregion

        #region IEnumerable Implementations
        public IEnumerator GetEnumerator() => targets?.Values?.GetEnumerator();
        IEnumerator<CombatTarget> IEnumerable<CombatTarget>.GetEnumerator() => targets?.Values?.GetEnumerator();
        #endregion

    }
}
