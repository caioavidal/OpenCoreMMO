using NeoServer.Game.Common;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Runes;
using NeoServer.Server.Model.Players.Contracts;
using System;
using System.Collections.Generic;

namespace NeoServer.Game.Items.Items.UsableItems.Runes
{
    public class AttackRune : Rune, IAttackRune
    {
        public AttackRune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes)
        {
        }
        public AttackRune(IItemType type, Location location, byte amount) : base(type, location, amount)
        {
        }
        public override ushort Duration => 2;

        public DamageType DamageType => DamageType.Energy;

        public ShootType ShootType => ShootType.EnergyBall;
        public bool NeedTarget => true;

        public void Use(IPlayer usedBy, ICreature creature)
        {
            if (creature is not ICombatActor enemy) return;

            var minMaxDamage = MinMaxFormula(usedBy);
            var damage = (ushort) GameRandom.Random.Next(minValue: minMaxDamage.Min, maxValue: minMaxDamage.Max);

            if(enemy.ReceiveAttack(usedBy, new Common.Combat.Structs.CombatDamage(damage, DamageType)))
            {
                Reduce();
            }
        }

        public void Use(IPlayer usedBy, IItem item)
        {
            if(NeedTarget == true)  return; 
        }
    }
}