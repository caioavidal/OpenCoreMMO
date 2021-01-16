using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types.Runes;
using NeoServer.Game.Contracts.World;
using NeoServer.Game.Contracts.World.Tiles;
using NeoServer.Game.Effects.Magical;
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
        public virtual EffectT Effect => Metadata.EffectT;
        public string Area => Metadata.Attributes.GetAttribute(ItemAttribute.Area);

        public bool NeedTarget => Metadata.Attributes.GetAttribute<bool>(ItemAttribute.NeedTarget);

        public virtual bool Use(ICreature usedBy, ICreature creature, out CombatAttackType combatAttackType)
        {
            if (NeedTarget == false ) return AttackArea(usedBy, creature.Tile, out combatAttackType);

            combatAttackType = CombatAttackType.None;

            if (creature is not ICombatActor enemy) return false;
            if (usedBy is not IPlayer player) return false;

            var minMaxDamage = Formula(player, player.Level, player.Skills[Common.Creatures.SkillType.Magic].Level);
            var damage = (ushort)GameRandom.Random.Next(minValue: minMaxDamage.Min, maxValue: minMaxDamage.Max);

            if (enemy.ReceiveAttack(player, new CombatDamage(damage, DamageType) { Effect = Effect }))
            {
                combatAttackType.ShootType = ShootType;
                combatAttackType.DamageType = DamageType;
                combatAttackType.EffectT = Effect;

                Reduce();
                return true;
            }

            return false;
        }

        public virtual bool Use(ICreature usedBy, ITile tile, out CombatAttackType combatAttackType)
        {
            return AttackArea(usedBy, tile, out combatAttackType);
        }
        private bool AttackArea(ICreature usedBy, ITile tile, out CombatAttackType combatAttackType)
        {
            combatAttackType = CombatAttackType.None;

            if (NeedTarget == true)
            {
                if(tile is IDynamicTile t && t.HasCreature) return Use(usedBy, t.TopCreatureOnStack, out combatAttackType);
                return false;
            }

            if (usedBy is not IPlayer player) return false;

            var minMaxDamage = Formula(player, player.Level, player.Skills[Common.Creatures.SkillType.Magic].Level);
            var damage = (ushort)GameRandom.Random.Next(minValue: minMaxDamage.Min, maxValue: minMaxDamage.Max);

            combatAttackType.DamageType = DamageType;
            combatAttackType.Area = AreaEffect.Create(tile.Location, Area);
            combatAttackType.EffectT = Effect;

            player.PropagateAttack(combatAttackType.Area, new CombatDamage(damage, DamageType));

            Reduce();
            return true;
        }

        public static new bool IsApplicable(IItemType type) => Rune.IsApplicable(type) && type.Attributes.HasAttribute(ItemAttribute.Damage);

    }
}