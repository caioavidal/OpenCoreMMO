using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Combat.Spells;
using NeoServer.Game.Common;
using NeoServer.Game.Common.Creatures.Players;
using NeoServer.Game.Contracts.Creatures;

namespace NeoServer.Game.Creatures.Spells
{
    public class ParalyzeSpell : Spell<ParalyzeSpell>
    {
        public override string Name => "Paralyze";
        public override EffectT Effect => EffectT.GlitterRed;
        public virtual ushort SpeedChange => 200;
        public override uint Duration => 10000;
        public override ushort Mana => 60;
        public override ConditionType ConditionType => ConditionType.Paralyze;

        private float MinA;
        private float MinB;
        private float MaxA;
        private float MaxB;

        public override bool OnCast(ICombatActor actor, string words, out InvalidOperation error)
        {
            error = InvalidOperation.None;

            var min = (actor.Speed * MinA) + MinB;
            var max = (actor.Speed * MaxA) + MaxB;

            actor.DecreaseSpeed(SpeedChange);
            return true;
        }
        public override void OnEnd(ICombatActor actor)
        {
            actor.IncreaseSpeed(SpeedChange);
            base.OnEnd(actor);
        }

        public void SetFormula(float minA, float minB, float maxA, float maxB)
        {
            MinA = minA;
            MinB = minB;
            MaxA = maxA;
            MaxB = maxB;
        }
    }
}
