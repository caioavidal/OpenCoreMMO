using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Game.Combat.Spells;

public abstract class AttackSpell : Spell<AttackSpell>
{
    public override EffectT Effect => EffectT.GlitterBlue;
    public override uint Duration => 0;
    public override ConditionType ConditionType => ConditionType.None;

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.None;

        return true;
        //actor.Attack()
    }
}