using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Helpers;

namespace NeoServer.Game.Creatures.Spells
{
    public class HealSpell : Spell<HealSpell>
    {
        public override string Name => "Healing";
        public override EffectT Effect { get; } = EffectT.GlitterBlue;
        public override ushort Mana => 60;
        public override ConditionType ConditionType => ConditionType.Haste;
        public virtual ushort Min { get; }
        public virtual ushort Max { get; }
        public override uint Duration => 0;

        public HealSpell(MinMax minMax, EffectT effect)
        {
            Effect = effect;
            Min = (ushort) minMax.Min;
            Max = (ushort) minMax.Max;
        }
        public override void OnCast(ICombatActor actor)
        {
            var hpToIncrease = GameRandom.Random.NextInRange(Min, Max);
            actor.Heal((ushort)hpToIncrease);
        }
        public override void OnEnd(ICombatActor actor)
        {
            base.OnEnd(actor);
        }
    }
}
