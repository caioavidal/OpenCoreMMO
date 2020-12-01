using NeoServer.Enums.Creatures.Enums;
using NeoServer.Game.Contracts.Creatures;
using NeoServer.Game.Common.Creatures.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeoServer.Game.Creatures.Spells
{
    public abstract class AttackSpell: Spell<AttackSpell>
    {
        public override EffectT Effect => EffectT.GlitterBlue;
        public override uint Duration => 0;
        public override ConditionType ConditionType => ConditionType.None;

        public override void OnCast(ICombatActor actor)
        {
            //actor.Attack()
        }
    }
}
