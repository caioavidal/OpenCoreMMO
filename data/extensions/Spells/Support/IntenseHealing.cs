using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Extensions.Spells.Support;

public class IntenseHealing : Spell<IntenseHealing>
{
    public override EffectT Effect => EffectT.GlitterBlue;
    public override uint Duration => 0;

    public override ConditionType ConditionType => ConditionType.None;

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.None;
        actor.Heal(100, actor);
        return true;
    }
}