using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
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
        public AttackRune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) : base(type, location, attributes) { }
        public AttackRune(IItemType type, Location location, byte amount) : base(type, location, amount) { }
        public override ushort Duration => 2;
        public virtual DamageType DamageType => Metadata.DamageType;
        public virtual ShootType ShootType => Metadata.ShootType;
        public bool NeedTarget => true;

        public bool Use(ICreature usedBy, ICreature creature, out CombatAttackType combatAttackType)
        {
            combatAttackType = CombatAttackType.None;

            if (creature is not ICombatActor enemy) return false;
            if (usedBy is not IPlayer player) return false;

            var minMaxDamage = Formula(player, player.Level, player.Skills[Common.Creatures.SkillType.Magic].Level);
            var damage = (ushort) GameRandom.Random.Next(minValue: minMaxDamage.Min, maxValue: minMaxDamage.Max);

            if(enemy.ReceiveAttack(player, new CombatDamage(damage, DamageType)))
            {
                combatAttackType.ShootType = ShootType;
                combatAttackType.DamageType = DamageType;

                Reduce();
                return true;
            }

            return false;
        }

        public void Use(IPlayer usedBy, IItem item)
        {
            if(NeedTarget == true)  return; 
        }
    }
}