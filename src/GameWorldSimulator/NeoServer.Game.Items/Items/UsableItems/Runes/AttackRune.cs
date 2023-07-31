using System;
using System.Collections.Generic;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Creatures;
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

    public override ushort Duration => 2;
    public bool HasNoInjureEffect => Metadata.Attributes.HasAttribute("hasnoinjureEffect");
    public string Area => Metadata.Attributes.GetAttribute(ItemAttribute.Area);
    public virtual ShootType ShootType => Metadata.ShootType;
    public virtual DamageType DamageType => Metadata.DamageType;
    public virtual EffectT Effect => Metadata.EffectT;

    public bool NeedTarget => Metadata.Attributes.GetAttribute<bool>(ItemAttribute.NeedTarget);

    public new static Func<IItem, ICombatActor, IThing, bool> UseFunction { get; set; }
    public virtual bool Use(ICombatActor aggressor, IThing victim, IItemCombatAttack combatAttack)
    {
        if (UseFunction?.Invoke(this, aggressor, victim) ?? false) return true;
        
        if (Amount <= 0) return false;
        
        var result = combatAttack.CauseDamage(this, aggressor, victim);
        
        if(result.Succeeded) Reduce();
        
        return result.Succeeded;
    }

    public CombatAttackParams PrepareAttack(IPlayer player)
    {
        var minMaxDamage = Formula(player, player.Level, player.GetSkillLevel(SkillType.Magic));
        var damage = (ushort)GameRandom.Random.Next(minMaxDamage.Min, maxValue: minMaxDamage.Max);

        return new CombatAttackParams(DamageType)
        {
            ShootType = ShootType,
            DamageType = DamageType,
            EffectT = Effect,
            Damages = new[]
            {
                new CombatDamage(damage, DamageType, HasNoInjureEffect)
            }
        };
    }
    
    public new static bool IsApplicable(IItemType type)
    {
        return type.Group is ItemGroup.AttackRune;
    }
}