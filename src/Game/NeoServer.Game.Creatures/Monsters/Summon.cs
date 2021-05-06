using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Creatures.Model.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoServer.Game.Creatures.Monsters
{
    public class Summon : Monster
    {
        public ICreature Master { get; }
        public override bool IsSummon => true;
        public Summon(IMonsterType type, ICreature master) : base(type, null)
        {
            Master = master;
            if(master is ICombatActor actor) actor.OnKilled +=  OnMasterKilled;
        }

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
            if (Master is ICombatActor actor) actor.OnKilled -=  OnMasterKilled;
        }

        private void OnMasterKilled(ICombatActor master, IThing by, ILoot loot)
        {
            master.OnKilled -= OnMasterKilled;
            Die();
        }
    }
}
