using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Creatures;

namespace NeoServer.Extensions.Spells.Support;

public class Invisibility : Spell<Invisibility>
{
    public override EffectT Effect => EffectT.GlitterBlue;
    public override uint Duration => 20000;
    public override ConditionType ConditionType => ConditionType.Invisible;

    public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
    {
        error = InvalidOperation.None;
        actor.TurnInvisible();
        return true;
    }

    public override void OnEnd(ICombatActor actor)
    {
        actor.TurnVisible();
        base.OnEnd(actor);
    }
}