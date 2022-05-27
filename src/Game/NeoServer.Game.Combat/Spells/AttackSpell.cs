using NeoServer.Game.Combat.Attacks;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Combat.Structs;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;
using NeoServer.Game.Common.Item;

namespace NeoServer.Game.Combat.Spells;

public abstract class AttackSpell : Spell<AttackSpell>
{
    public override EffectT Effect => EffectT.GlitterBlue;
    public override uint Duration => 0;
    public abstract DamageType DamageType { get; }
    public override ConditionType ConditionType => ConditionType.None;
    public abstract CombatAttack CombatAttack { get; }
    public abstract MinMax Damage { get; }
    public virtual byte Range => 0; 

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.None;

        var target = actor.AutoAttackTarget as ICombatActor; 

        return actor.Attack(target, CombatAttack, new CombatAttackValue
        {
            Range = Range,
            DamageType = DamageType,
            MaxDamage = (ushort)Damage.Max,
            MinDamage = (ushort)Damage.Min
        });
    }
}