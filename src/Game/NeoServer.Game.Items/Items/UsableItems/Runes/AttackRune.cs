using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Contracts.World.Tiles;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Effects.Magical;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;

namespace NeoServer.Game.Items.Items.UsableItems.Runes;

public class AttackRune : Rune, IAttackRune
{
    internal AttackRune(IItemType type, Location location, IDictionary<ItemAttribute, IConvertible> attributes) :
        base(type, location, attributes)
    {
    }

    public Func<string, byte[,]> GetAreaTypeFunc { get; init; }
    public override ushort Duration => 2;
    public bool HasNoInjureEffect => Metadata.Attributes.HasAttribute("hasnoinjureEffect");
    public string Area => Metadata.Attributes.GetAttribute(ItemAttribute.Area);
    public virtual ShootType ShootType => Metadata.ShootType;
    public virtual DamageType DamageType => Metadata.DamageType;
    public virtual EffectT Effect => Metadata.EffectT;

    public bool NeedTarget => Metadata.Attributes.GetAttribute<bool>(ItemAttribute.NeedTarget);

    public virtual bool Use(ICreature usedBy, ICreature creature, out CombatAttackType combatAttackType)
    {
        if (NeedTarget == false) return AttackArea(usedBy, creature.Tile, out combatAttackType);

        combatAttackType = CombatAttackType.None;

        if (creature is not ICombatActor enemy) return false;
        if (usedBy is not IPlayer player) return false;

        var minMaxDamage = Formula(player, player.Level, player.GetSkillLevel(SkillType.Magic));
        var damage = (ushort)GameRandom.Random.Next(minMaxDamage.Min, maxValue: minMaxDamage.Max);

        if (enemy.ReceiveAttack(player, new CombatDamage(damage, DamageType, HasNoInjureEffect)))
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

        if (NeedTarget)
        {
            if (tile is IDynamicTile { HasCreature: true } t)
                return Use(usedBy, t.TopCreatureOnStack, out combatAttackType);
            return false;
        }

        if (usedBy is not IPlayer player) return false;

        var minMaxDamage = Formula(player, player.Level, player.GetSkillLevel(SkillType.Magic));
        var damage = (ushort)GameRandom.Random.Next(minMaxDamage.Min, maxValue: minMaxDamage.Max);

        combatAttackType.DamageType = DamageType;

        var template = GetAreaTypeFunc?.Invoke(Area);
        combatAttackType.SetArea(AreaEffect.Create(tile.Location, Area, template));

        combatAttackType.EffectT = Effect;

        player.PropagateAttack(combatAttackType.Area, new CombatDamage(damage, DamageType, HasNoInjureEffect));

        Reduce();
        return true;
    }

    public new static bool IsApplicable(IItemType type)
    {
        return Rune.IsApplicable(type) && type.Attributes.HasAttribute(ItemAttribute.Damage);
    }
}