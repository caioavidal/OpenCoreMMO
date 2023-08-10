using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Combat.Attacks;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Contracts.Spells;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Helpers;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Results;
using NeoServer.Game.Common.Services;

namespace NeoServer.Game.Combat.Spells;

public abstract class AttackSpell : Spell<AttackSpell>, IAttackSpell
{
    public override EffectT Effect => EffectT.GlitterBlue;
    public override uint Duration => 0;
    public virtual DamageType DamageType => DamageType.Physical;
    public virtual EffectT AreaEffect => EffectT.None;
    public override ConditionType ConditionType => ConditionType.None;
    public virtual byte Range => 0;
    public virtual bool NeedsTarget => false;
    public virtual ShootType ShootType => ShootType.None;
    public virtual string AreaName { get; }
    public virtual MinMax GetFormula(ICombatActor player) => new(0, 0);

    public virtual CombatAttackParams PrepareAttack(ICombatActor actor)
    {
        var minMax = GetFormula(actor);
        var damage = GameRandom.Random.NextInRange(minMax);

        return new CombatAttackParams
        {
            DamageType = DamageType,
            ShootType = ShootType,
            EffectT = AreaEffect,
            AreaName = AreaName,
            Damages = new[]
            {
                new CombatDamage((ushort)damage, DamageType),
            }
        };
    }
    public abstract ISpellCombatAttack CombatAttack { get; }

    public override Result CanCast(ICombatActor actor)
    {
        if (actor is not IPlayer player) return Result.NotApplicable;

        if (!NeedsTarget) return Result.Success;

        if (actor.CurrentTarget is not ICombatActor enemy)
        {
            OperationFailService.Send(player, "You need to set a target to use this spell.");
            return Result.NotPossible;
        }

        if (!DistanceCombatAttack.CanAttack(actor, enemy, maxRange: Range))
        {
            OperationFailService.Send(player, "You are too far from target.");
            return Result.NotPossible;
        }

        return Result.Success;
    }

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.NotPossible;

        if (Guard.AnyNull(actor, words)) return false;
        
        if (actor.CurrentTarget is not null && actor.CurrentTarget is not ICombatActor) return false;

        if (CombatAttack is null) return false;

        var result = CombatAttack.CauseDamage(this, actor, actor.CurrentTarget as ICombatActor);

        if (result.Succeeded) return true;

        error = result.Error;
        return false;
    }
}