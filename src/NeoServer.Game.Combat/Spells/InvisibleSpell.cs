using NeoServer.Enums.Creatures.Enums;
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

        public override void OnCast(ICombatActor actor)
        {
            actor.TurnInvisible();
        }
        public override void OnEnd(ICombatActor actor)
        {
            actor.TurnVisible();
            base.OnEnd(actor);
        }
    }
}
