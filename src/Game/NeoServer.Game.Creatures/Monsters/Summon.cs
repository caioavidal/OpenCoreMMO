using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.World;

namespace NeoServer.Game.Creatures.Monsters
{
    public class Summon : Monster
    {
        public Summon(IMonsterType type, IMapTool mapTool, ICreature master) : base(type, mapTool, null)
        {
            Master = master;
            if (master is ICombatActor actor)
            {
                actor.OnKilled += OnMasterKilled;
                actor.OnTargetChanged += OnMasterTargetChange;
            }
        }

        public ICreature Master { get; }
        public override bool IsSummon => true;

        public override void SetAsEnemy(ICreature creature)
        {
            if (Master == creature) return;

            if (creature is Summon summon && summon.Master == Master) return;

            base.SetAsEnemy(creature);
        }

        public void Die()
        {
            OnDeath(this);
        }
        public override void OnDeath(IThing by)
        {
            base.OnDeath(by);

            if (Master is ICombatActor actor)
            {
                actor.OnKilled -= OnMasterKilled;
                actor.OnTargetChanged -= OnMasterTargetChange;
            }
        }

        private void OnMasterKilled(ICombatActor master, IThing by, ILoot loot)
        {
            master.OnKilled -= OnMasterKilled;
            master.OnTargetChanged -= OnMasterTargetChange;
            Die();
        }

        private void OnMasterTargetChange(ICombatActor actor, uint oldTargetId, uint newTargetId)
        {
            Targets.Clear();
            SetAsEnemy(actor.AutoAttackTarget);
        }
    }
}