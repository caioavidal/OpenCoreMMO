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
        private HasteSpell() { }
        public override EffectT Effect => EffectT.GlitterBlue;

        private void Action(ICreature actor, ushort speed)
        {
            actor.IncreaseSpeed(speed);
        }

        public void Remove(ICreature actor, ushort speed)
        {
            actor.IncreaseSpeed(speed);
        }

        public void Invoke(ICreature actor, ushort speedBoost, uint duration)
        {
            if (actor.HasCondition(ConditionType.Haste, out var condition))
            {
                actor.AddCondition(condition);
                return;
            }

            var hasteCondition = new HasteCondition(duration);

            hasteCondition.OnEnd = () => Remove(actor, speedBoost);

            actor.AddCondition(hasteCondition);

            Action(actor, speedBoost);
        }
    }
}
