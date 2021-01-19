using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Spells
{
    public class InvisibleSpell: Spell<InvisibleSpell>
    {
        public override string Name => "Invisible";
        public override EffectT Effect { get; } = EffectT.GlitterBlue;
        public override uint Duration { get; } = 10000;
        public override ushort Mana => 60;
        public override ConditionType ConditionType => ConditionType.Invisible;
        public InvisibleSpell(uint duration)
        {
            Duration = duration;
        }
        public InvisibleSpell(uint duration, EffectT effect)
        {
            Duration = duration;
            Effect = effect;
        }

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
}
