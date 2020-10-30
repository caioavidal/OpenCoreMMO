using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Enums.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Spells
{
    public class HasteSpell: Spell<HasteSpell>
    {
        public HasteSpell() { }
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 10000;
        public virtual ushort SpeedBoost => 200;

        public virtual void OnCast(ICombatActor actor, ushort speed)
        {
            actor.IncreaseSpeed(speed);
        }

        public void Remove(ICombatActor actor, ushort speed)
        {
            actor.IncreaseSpeed(speed);
        }
        public override void Invoke(ICombatActor actor)
        {
            Invoke(actor, SpeedBoost, Duration);
        }
        public void Invoke(ICombatActor actor, ushort speedBoost, uint duration)
        {
            if (actor.HasCondition(ConditionType.Haste, out var condition))
            {
                actor.AddCondition(condition);
                return;
            }

            var hasteCondition = new HasteCondition(duration);

            hasteCondition.OnEnd = () => Remove(actor, speedBoost);

            actor.AddCondition(hasteCondition);            

            OnCast(actor, speedBoost);
        }
    }
}
