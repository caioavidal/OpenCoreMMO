﻿using System.Linq;
using NeoServer.Game.Common.Contracts;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;

namespace NeoServer.Game.Creatures.Events.Monsters
{
    public class MonsterKilledEventHandler : IGameEventHandler
    {
        public void Execute(ICreature creature, IThing by, ILoot loot)
        {
            if (creature is not IMonster monster) return;
            GiveExperience(monster);
        }

        private void GiveExperience(IMonster monster)
        {
            if (monster.IsSummon) return;

            var totalDamage = monster.Damages.Sum(x => x.Value);

            foreach (var enemyDamage in monster.Damages)
            {
                var damage = enemyDamage.Value;

                var damagePercent = damage * 100 / totalDamage;

                var exp = damagePercent * monster.Experience / 100;

                if (enemyDamage.Key is not ICombatActor actor) continue;
                actor.GainExperience((uint) exp);
            }
        }
    }
}