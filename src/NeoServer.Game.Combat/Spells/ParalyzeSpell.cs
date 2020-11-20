using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

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

        public override void OnCast(ICombatActor actor)
        {
            var min = (actor.Speed * MinA) + MinB;
            var max = (actor.Speed * MaxA) + MaxB;

            actor.DecreaseSpeed(SpeedChange);
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
