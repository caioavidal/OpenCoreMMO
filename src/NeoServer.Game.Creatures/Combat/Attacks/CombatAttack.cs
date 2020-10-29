using NeoServer.Game.Contracts.Combat;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Creatures.Combat.Attacks;
using NeoServer.Game.Enums.Item;
using NeoServer.Server.Helpers;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Creatures.Model.Monsters
{

    public class CombatAttack : ICombatAttack
    {
        public CombatAttack(DamageType damageType, CombatAttackOption option)
        {
            option.DamageType = damageType;
            Option = option;
        }

        public CombatAttackOption Option { get; }

        //todo code smell
        public virtual void BuildAttack(ICombatActor actor, ICombatActor enemy) { }

        public virtual ushort CalculateDamage(ushort attackPower, ushort minAttackPower) => (ushort)GaussianRandom.Random.NextInRange(minAttackPower, attackPower);


        public virtual void CauseDamage(ICombatActor actor, ICombatActor enemy)
        {
            enemy.ReceiveAttack(actor, this, CalculateDamage(actor.AttackPower, actor.MinimumAttackPower));
        }
    }


}
