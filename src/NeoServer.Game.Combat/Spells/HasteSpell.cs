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
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 10000;
        public virtual ushort SpeedBoost => 200;
        public override ushort Mana => 60;
        public override ConditionType ConditionType => ConditionType.Haste;

        public override void OnCast(ICombatActor actor)
        {
            actor.IncreaseSpeed(SpeedBoost);
        }
        public override void OnEnd(ICombatActor actor)
        {
            actor.DecreaseSpeed(SpeedBoost);
            base.OnEnd(actor);
        }
    }
}
